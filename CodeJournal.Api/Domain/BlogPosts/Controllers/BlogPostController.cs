using CodeJournal.Api.Domain.AccountManagement.Models;
using CodeJournal.Api.Domain.BlogPosts.Dtos;
using CodeJournal.Api.Domain.BlogPosts.Models;
using CodeJournal.Api.Domain.BlogPosts.Repositories;
using CodeJournal.Api.Domain.Categories;
using CodeJournal.Api.Domain.Categories.Dtos;
using CodeJournal.Api.Domain.Categories.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeJournal.Api.Domain.BlogPosts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBlogPostLikeRepository _blogPostLikeRepository;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IBlogPostCommentRepository blogPostCommentRepository;

        public BlogPostController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository, IBlogPostLikeRepository blogPostLikeRepository, IBlogPostCommentRepository blogPostCommentRepository, UserManager<ApplicationUser> userManager)
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
            _blogPostLikeRepository = blogPostLikeRepository;
            this.blogPostCommentRepository = blogPostCommentRepository;
            this.userManager = userManager;
        }

        // POST: {apibaseurl}/api/blogpost
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateBlogPost(CreateBlogPostRequestDto request)
        {
            // map dto to domain model 

            var blogPost = new BlogPost()
            {
                Title = request.Title,
                ShortDescription = request.ShortDescription,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                UrlHandle = request.UrlHandle,
                PublishedDate = request.PublishedDate,
                Author = request.Author,
                IsVisible = request.IsVisible,
                Categories = new List<Category>() // Assuming you have a way to get categories from the request
            };

            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(categoryGuid);

                if (existingCategory is not null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            blogPost = await _blogPostRepository.CreateAsync(blogPost);

            // map domain model to dto
            var response = new BlogPostDto()
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                UrlHandle = blogPost.UrlHandle,
                PublishedDate = blogPost.PublishedDate,
                Author = blogPost.Author,
                IsVisible = blogPost.IsVisible,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };

            return Ok(response);

        }

        // GET: {apibaseurl}/api/blogpost
        [HttpGet]
        public async Task<IActionResult> GetBlogPosts()
        {
            var blogPosts = await _blogPostRepository.GetAllAsync();


            var response = new List<BlogPostDto>();
            foreach (var blogPost in blogPosts)
            {
                response.Add(new BlogPostDto()
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    ShortDescription = blogPost.ShortDescription,
                    Content = blogPost.Content,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    PublishedDate = blogPost.PublishedDate,
                    Author = blogPost.Author,
                    IsVisible = blogPost.IsVisible,
                    Categories = blogPost.Categories.Select(x => new CategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle
                    }).ToList()
                });
            }

            return Ok(response);
        }

        //GET: {apibaseurl}/api/blogpost/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            //get blog post from Repository


            var blogPost = await _blogPostRepository.GetByIdAsync(id);

            if (blogPost is null)
            {
                return NotFound();
            }

            // Convert the domain model to DTO
            var response = new BlogPostDto()
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                UrlHandle = blogPost.UrlHandle,
                PublishedDate = blogPost.PublishedDate,
                Author = blogPost.Author,
                IsVisible = blogPost.IsVisible,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };

            return Ok(response);
        }

        //Get: {apibaseurl}/api/blogpost/{urlHandle}
        [HttpGet]
        [Route("{urlHandle}")]
        public async Task<IActionResult> GetblogPostByUrlHandle([FromRoute] string urlHandle, string? userId = null)
        {
            //get blog post from repository by url handle

            var blogPost = await _blogPostRepository.GetByUrlHandleAsync(urlHandle);


            if (blogPost is null)
            {
                return NotFound();
            }
            bool liked = false;

            if (!string.IsNullOrEmpty(userId))
            {
                var likes = await _blogPostLikeRepository.GetLikesForBlog(blogPost.Id);

                liked = likes.Any(x => x.UserId == userId);

            }
            var totalLikes = await _blogPostLikeRepository.GetTotalLikesForBlog(blogPost.Id);

            // Convert the domain model to DTO
            var response = new BlogPostDto()
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                UrlHandle = blogPost.UrlHandle,
                PublishedDate = blogPost.PublishedDate,
                Author = blogPost.Author,
                IsVisible = blogPost.IsVisible,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList(),
                TotalLikes = totalLikes,
                Liked = liked
            };

            return Ok(response);
        }
        [HttpGet]
        [Route("{blogPostId:Guid}/comments")]
        public async Task<IActionResult> GetBlogPostComments([FromRoute] Guid blogPostId)
        {
            var comments = await blogPostCommentRepository.GetAllAsync(blogPostId);

            var response = new List<BlogCommentDto>();

            foreach (var comment in comments)
            {
                var user = await userManager.FindByIdAsync(comment.UserId.ToString());

                response.Add(new BlogCommentDto
                {
                    Description = comment.Description,
                    DateAdded = comment.DateAdded,
                    UserName = user?.UserName ?? "Unknown"
                });
            }

            return Ok(response);
        }

        // PUT: {apibaseurl}/api/blogpost/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, [FromBody] UpdateBlogPostRequestDto request)
        {

            var blogPost = new BlogPost()
            {
                Id = id,
                Title = request.Title,
                ShortDescription = request.ShortDescription,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                UrlHandle = request.UrlHandle,
                PublishedDate = request.PublishedDate,
                Author = request.Author,
                IsVisible = request.IsVisible,
                Categories = new List<Category>() // Assuming you have a way to get categories from the request
            };

            //For each 
            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await _categoryRepository.GetByIdAsync(categoryGuid);

                if (existingCategory is not null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            // call respository to update blog post

            var updatedBlogPost = await _blogPostRepository.UdpateAsync(blogPost);

            if (updatedBlogPost is null)
            {
                return NotFound();
            }
            var respone = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                UrlHandle = blogPost.UrlHandle,
                PublishedDate = blogPost.PublishedDate,
                Author = blogPost.Author,
                IsVisible = blogPost.IsVisible,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()

            };

            return Ok(respone);

        }

        // DELETE: {apibaseurl}/api/blogpost/{id}

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id)
        {
            var deletedBlogPost = await _blogPostRepository.DeleteAsync(id);
            if (deletedBlogPost is null)
            {
                return NotFound();
            }
            // Convert  domain model to dto
            var response = new BlogPostDto()
            {
                Id = deletedBlogPost.Id,
                Title = deletedBlogPost.Title,
                ShortDescription = deletedBlogPost.ShortDescription,
                Content = deletedBlogPost.Content,
                FeaturedImageUrl = deletedBlogPost.FeaturedImageUrl,
                UrlHandle = deletedBlogPost.UrlHandle,
                PublishedDate = deletedBlogPost.PublishedDate,
                Author = deletedBlogPost.Author,
                IsVisible = deletedBlogPost.IsVisible,
                Categories = deletedBlogPost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };

            return Ok(response);



        }

        [HttpPost]
        [Route("comment")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] AddBlogPostCommentDto request)
        {
            if (request == null || request.BlogPostId == Guid.Empty)
                return BadRequest("Invalid comment data.");


            var comment = new BlogPostComment()
            {
                BlogPostId = request.BlogPostId,
                UserId = request.UserId,
                Description = request.Description,
                DateAdded = DateTimeOffset.UtcNow

            };
            var createdComment = await blogPostCommentRepository.AddAsync(comment);
            var response = new BlogPostCommentDto
            {
                Id = createdComment.Id,
                BlogPostId = createdComment.BlogPostId,
                UserId = createdComment.UserId,
                Description = createdComment.Description,
                DateAdded = createdComment.DateAdded
            };

            return Ok(response);

        }

    }


}
