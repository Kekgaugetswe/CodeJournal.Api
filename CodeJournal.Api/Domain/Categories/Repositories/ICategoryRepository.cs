using System;
using CodeJournal.Api.Domain.Categories.Dtos;

namespace CodeJournal.Api.Domain.Categories.Repositories;

public interface ICategoryRepository
{
    Task<Category> CreateCategoryAsync(Category category);
    Task<IEnumerable<Category>> GetAllAsync(string? query = null);

    Task<Category?> GetByIdAsync(Guid id);

    Task<Category> UpdateAsync(Category category);

    Task<Category>  DeleteAsync(Guid id);

}
 