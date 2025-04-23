using System;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.BlogPosts.Models;

namespace CodeJournal.Api.Domain.BlogPosts.Repositories;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly ApplicationDbContext context;

    public BlogPostRepository(ApplicationDbContext context)
    {
        this.context = context;
        
    }
    public async Task<BlogPost> CreateAsync(BlogPost blogPost)
    {
        await context.BlogPosts.AddAsync(blogPost);
        await context.SaveChangesAsync();
        return blogPost;
        
    }
}
