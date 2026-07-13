using System.Security.Claims;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.BlogPosts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.Domain.BlogPosts.Controllers;

[Route("api/comments")]
[ApiController]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public CommentController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // POST: api/comments/{commentId}/likes
    [HttpPost("{commentId:Guid}/likes")]
    public async Task<IActionResult> LikeComment([FromRoute] Guid commentId)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var comment = await _dbContext.BlogPostComment
            .Include(c => c.Likes)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null) return NotFound();
        if (comment.IsDeleted) return BadRequest("Cannot like a deleted comment.");

        // Check if already liked
        var existingLike = comment.Likes.FirstOrDefault(l => l.UserId == userId.Value);
        if (existingLike != null)
        {
            return Ok(new
            {
                commentId = comment.Id,
                likeCount = comment.Likes.Count,
                isLikedByCurrentUser = true
            });
        }

        var like = new CommentLike
        {
            Id = Guid.NewGuid(),
            CommentId = commentId,
            UserId = userId.Value,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _dbContext.CommentLikes.Add(like);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            commentId = comment.Id,
            likeCount = comment.Likes.Count + 1,
            isLikedByCurrentUser = true
        });
    }

    // DELETE: api/comments/{commentId}/likes
    [HttpDelete("{commentId:Guid}/likes")]
    public async Task<IActionResult> UnlikeComment([FromRoute] Guid commentId)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var comment = await _dbContext.BlogPostComment
            .Include(c => c.Likes)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null) return NotFound();

        var existingLike = comment.Likes.FirstOrDefault(l => l.UserId == userId.Value);
        if (existingLike == null)
        {
            return Ok(new
            {
                commentId = comment.Id,
                likeCount = comment.Likes.Count,
                isLikedByCurrentUser = false
            });
        }

        _dbContext.CommentLikes.Remove(existingLike);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            commentId = comment.Id,
            likeCount = comment.Likes.Count - 1,
            isLikedByCurrentUser = false
        });
    }

    // DELETE: api/comments/{commentId}
    [HttpDelete("{commentId:Guid}")]
    public async Task<IActionResult> DeleteComment([FromRoute] Guid commentId)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var comment = await _dbContext.BlogPostComment.FindAsync(commentId);
        if (comment == null) return NotFound();
        if (comment.IsDeleted) return BadRequest("Comment is already deleted.");

        // Check permission: user owns comment OR has Writer role
        var isOwner = comment.UserId == userId.Value;
        var isWriter = User.IsInRole("Writer");

        if (!isOwner && !isWriter)
        {
            return Forbid();
        }

        comment.IsDeleted = true;
        comment.Description = string.Empty;
        comment.DeletedAt = DateTimeOffset.UtcNow;
        comment.DeletedByUserId = userId.Value;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }
        return userId;
    }
}
