using System;
using CodeJournal.Api.Domain.Categories.Dtos;

namespace CodeJournal.Api.Domain.Categories.Repositories;

public interface ICategoryRepository
{
    Task<Category> CreateCategoryAsync(Category category);
    Task<IEnumerable<Category>> GetAllAsync();

}
