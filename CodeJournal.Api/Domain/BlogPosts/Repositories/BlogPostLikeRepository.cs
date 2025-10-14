using System;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.BlogPosts.Dtos;
using CodeJournal.Api.Domain.BlogPosts.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.Domain.BlogPosts.Repositories;

public class BlogPostLikeRepository : IBlogPostLikeRepository
{
    private readonly ApplicationDbContext dbContext;

    public BlogPostLikeRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<bool> AddLikeForBlog(AddBlogPostRequestDto request)
    {

        var exists = await dbContext.BlogPostLike.AnyAsync(l => l.BlogPostId == request.BlogPostId && l.UserId == request.UserId);
        if (exists)
            return false;
        var like = new BlogPostLike
        {
            Id = Guid.NewGuid(),
            BlogPostId = request.BlogPostId,
            UserId = request.UserId,
            LikedAt = DateTime.UtcNow
        };
        await dbContext.AddAsync(like);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<BlogPostLike>> GetLikesForBlog(Guid blogPostId)
    {
        return await dbContext.BlogPostLike.Where(x => x.BlogPostId == blogPostId).ToListAsync();
    }

    public async Task<int> GetTotalLikesForBlog(Guid BlogPostId)
    {
        return await dbContext.BlogPostLike.CountAsync(x => x.BlogPostId == BlogPostId);

    }
}
