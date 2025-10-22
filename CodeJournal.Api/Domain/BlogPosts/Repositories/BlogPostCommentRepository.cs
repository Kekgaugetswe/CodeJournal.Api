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
        return await dbContext.BlogPostComment.OrderByDescending(c=> c.DateAdded).Where(x => x.BlogPostId == blogPostId).ToListAsync();
    }
}
