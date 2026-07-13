using System;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.BlogPosts.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.Domain.BlogPosts.Repositories;

public class BlogPostCommentRepository : IBlogPostCommentRepository
{
    private readonly ApplicationDbContext dbContext;

    public BlogPostCommentRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment)
    {
        await dbContext.BlogPostComment.AddAsync(blogPostComment);
        await dbContext.SaveChangesAsync();

        return blogPostComment;
    }

    public async Task<IEnumerable<BlogPostComment>> GetAllAsync(Guid blogPostId)
    {
        // Return only top-level comments with their replies and likes loaded
        return await dbContext.BlogPostComment
            .Where(x => x.BlogPostId == blogPostId && x.ParentCommentId == null)
            .Include(x => x.Likes)
            .Include(x => x.Replies)
                .ThenInclude(r => r.Likes)
            .OrderByDescending(c => c.DateAdded)
            .ToListAsync();
    }

    public async Task<BlogPostComment?> GetByIdAsync(Guid id)
    {
        return await dbContext.BlogPostComment.FindAsync(id);
    }
}
