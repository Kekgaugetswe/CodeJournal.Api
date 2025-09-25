using System;
using CodeJournal.Api.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.Domain.BlogPosts.Repositories;

public class BlogPostLikeRepository : IBlogPostLikeRepository
{
    private readonly ApplicationDbContext dbContext;

    public BlogPostLikeRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<int> GetTotalLikesForBlog(Guid BlogPostId)
    {
        return await dbContext.BlogPostLike.CountAsync(x => x.BlogPostId == BlogPostId);

    }
}
