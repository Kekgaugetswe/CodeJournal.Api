using System;
using CodeJournal.Api.Domain.Categories.Dtos;

namespace CodeJournal.Api.Domain.Categories.Repositories;

public interface ICategoryRepository
{
    Task<Category> CreateCategoryAsync(Category category);
    Task<IEnumerable<Category>> GetAllAsync(string? query = null, string? sortBy = null, string? sortDirection = null, int? pageNumber = 1, int? pageSize = 100);

    Task<Category?> GetByIdAsync(Guid id);

    Task<Category> UpdateAsync(Category category);

    Task<Category> DeleteAsync(Guid id);

    Task<int> GetCount();

}
