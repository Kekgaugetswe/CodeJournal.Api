using System;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.BlogPosts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;

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

    public async Task<BlogPost?> DeleteAsync(Guid id)
    {
        await context.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == id);
        var exisitingBlogPost = await context.BlogPosts.FindAsync(id);
        if (exisitingBlogPost is not null)
        {
            context.BlogPosts.Remove(exisitingBlogPost);
            await context.SaveChangesAsync();
            return exisitingBlogPost;
        }
        return null;


    }

    public async Task<IEnumerable<BlogPost>> GetAllAsync()
    {
        return await context.BlogPosts.Include(x => x.Categories).ToListAsync();
    }

    public async Task<BlogPost?> GetByIdAsync(Guid id)
    {
        return await context.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<BlogPost?> UdpateAsync(BlogPost blogPost)
    {
        var existingBlogPost = await context.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == blogPost.Id);

        if (existingBlogPost is null)
        {
            return null;
        }
        // UpdateAdapter blog
        context.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);

        //update categories

        existingBlogPost.Categories = blogPost.Categories;
        await context.SaveChangesAsync();
        return blogPost;

    }

}
