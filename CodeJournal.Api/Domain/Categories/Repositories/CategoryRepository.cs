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



    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await context.Categories.ToListAsync();

    }

    public async Task<Category?> GetById(Guid id)
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
