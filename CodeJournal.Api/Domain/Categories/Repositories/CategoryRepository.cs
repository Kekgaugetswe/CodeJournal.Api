using System;
using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.Categories.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.Domain.Categories.Repositories;

public class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{

    public async Task<Category> CreateCategoryAsync(Category category)
    {

        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        return category;
    }



    public async Task<IEnumerable<Category>> GetAllAsync(string? query = null, string? sortBy = null, string? sortDirection = null, int? pageNumber = 1, int? pageSize = 100)
    {

        //Query

        var categories = context.Categories.AsQueryable();

        // Filtering
        if (string.IsNullOrWhiteSpace(query) == false)
        {
            categories = categories.Where(x => x.Name.Contains(query));
        }
        // sorting

        if (string.IsNullOrWhiteSpace(sortBy) == false)
        {
            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;

                categories = isAsc ? categories.OrderBy(x => x.Name) : categories.OrderByDescending(x => x.Name);
            }
        }

        if (string.IsNullOrWhiteSpace(sortBy) == false)
        {
            if (sortBy.Equals("URL", StringComparison.OrdinalIgnoreCase))
            {
                var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;

                categories = isAsc ? categories.OrderBy(x => x.UrlHandle) : categories.OrderByDescending(x => x.UrlHandle);
            }
        }

        // pagination
        //pageNumber 1 pagesize 5 - skip 0, take 5 
        //pageNumber 2 pagesize 5 - skip 5, take 5
        //pageNumber 3 pagesize 5 - skip 10, take 5

        var skipResults = (pageNumber - 1) * pageSize;

        categories = categories.Skip(skipResults ?? 0).Take(pageSize ?? 100);
        
        return await categories.ToListAsync();


    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

    }

    public async Task<Category> UpdateAsync(Category category)
    {
        var existingCategory = await context.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);
        if (existingCategory != null)
        {
            context.Entry(existingCategory).CurrentValues.SetValues(category);
            await context.SaveChangesAsync();
            return category;
        }
        return null;
    }

    public async Task<Category> DeleteAsync(Guid id)
    {
        var existingCategory = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (existingCategory is null)
        {
            return null;
        }
        context.Categories.Remove(existingCategory);
        await context.SaveChangesAsync();
        return existingCategory;
    }

}
