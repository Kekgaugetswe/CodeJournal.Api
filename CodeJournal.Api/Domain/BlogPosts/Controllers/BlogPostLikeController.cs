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

        var added = await _blogPostLikeRepository.AddLikeForBlog(request);
        if (!added)
            return Conflict(new { message = "User already liked this post" });
        return Ok(new { message = "Like Added." });
    }

    [HttpGet]
    [Route("{blogPostId:Guid}/totalLikes")]
    public async Task<IActionResult> GetTotalLikes([FromRoute] Guid blogPostId)
    {
        var totalLikes = await _blogPostLikeRepository.GetTotalLikesForBlog(blogPostId);
        return Ok(totalLikes);
    }
 
}