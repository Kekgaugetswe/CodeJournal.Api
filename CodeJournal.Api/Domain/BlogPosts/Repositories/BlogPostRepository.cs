using System;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.BlogPosts.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<BlogPost>> GetAllAsync()
    {
       return await context.BlogPosts.Include(x => x.Categories).ToListAsync();
    }

    public async Task<BlogPost?> GetByIdAsync(Guid id)
    {
      return  await context.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == id);
    }
}
