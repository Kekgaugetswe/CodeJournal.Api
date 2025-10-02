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

    public async Task AddLikeForBlog(AddBlogPostRequestDto request)
    {
        var like = new BlogPostLike
        {
            Id = Guid.NewGuid(),
            BlogPostId = request.BlogPostId,
            UserId = request.UserId
        };
        await dbContext.AddAsync(like);
        await dbContext.SaveChangesAsync();
    }

    public async Task<int> GetTotalLikesForBlog(Guid BlogPostId)
    {
        return await dbContext.BlogPostLike.CountAsync(x => x.BlogPostId == BlogPostId);

    }
}
