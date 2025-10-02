using CodeJournal.Api.Domain.BlogPosts.Dtos;
using CodeJournal.Api.Domain.BlogPosts.Models;
using CodeJournal.Api.Domain.BlogPosts.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeJournal.Api.Domain.BlogPosts.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlogPostLikeController : ControllerBase
{
    private readonly IBlogPostLikeRepository _blogPostLikeRepository;

    public BlogPostLikeController(IBlogPostLikeRepository blogPostLikeRepository)
    {
        _blogPostLikeRepository = blogPostLikeRepository;
    }

    [Authorize]
    [HttpPost]
    [Route("Add")]
    public async Task<IActionResult> AddLike([FromBody] AddBlogPostRequestDto request)
    {

        await _blogPostLikeRepository.AddLikeForBlog(request);
        return Ok();
    }
 
}
