using System;
using CodeJournal.Api.Common.Models;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.BlogPosts.Dtos;
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

    public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
    {
        return await context.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
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

    public async Task<PagedResult<BlogPost>> GetPagedAsync(BlogPostFilterParameters filterParameters)
    {
        var query = context.BlogPosts.Include(x => x.Categories).AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filterParameters.Title))
        {
            query = query.Where(x => EF.Functions.ILike(x.Title, $"%{filterParameters.Title}%"));
        }

        if (!string.IsNullOrWhiteSpace(filterParameters.Author))
        {
            query = query.Where(x => EF.Functions.ILike(x.Author, filterParameters.Author));
        }

        if (filterParameters.CategoryId.HasValue)
        {
            query = query.Where(x => x.Categories.Any(c => c.Id == filterParameters.CategoryId.Value));
        }

        if (filterParameters.IsVisible.HasValue)
        {
            query = query.Where(x => x.IsVisible == filterParameters.IsVisible.Value);
        }

        // Sorting
        if (!string.IsNullOrWhiteSpace(filterParameters.SortBy))
        {
            var isAsc = string.Equals(filterParameters.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);
            query = filterParameters.SortBy.ToLower() switch
            {
                "title" => isAsc ? query.OrderBy(x => x.Title) : query.OrderByDescending(x => x.Title),
                "author" => isAsc ? query.OrderBy(x => x.Author) : query.OrderByDescending(x => x.Author),
                "date" or "publisheddate" => isAsc ? query.OrderBy(x => x.PublishedDate) : query.OrderByDescending(x => x.PublishedDate),
                _ => query.OrderByDescending(x => x.PublishedDate)
            };
        }
        else
        {
            // Default ordering: PublishedDate descending (latest first)
            query = query.OrderByDescending(x => x.PublishedDate);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((filterParameters.PageNumber - 1) * filterParameters.PageSize)
            .Take(filterParameters.PageSize)
            .ToListAsync();

        return new PagedResult<BlogPost>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

}
