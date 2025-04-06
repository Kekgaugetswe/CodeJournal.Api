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
       return  await context.Categories.ToListAsync();

    }
}
