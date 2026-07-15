using CodeJournal.Api.DataAccess;
using CodeJournal.Api.Domain.Categories.Dtos;
using CodeJournal.Api.Domain.Categories.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeJournal.Api.Domain.Categories.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryRepository repository, ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> CreateCategory(CreateCategoryRequestDto request)
    {
        // map dto to domain model 

        var category = new Category()
        {
            Name = request.Name,
            UrlHandle = request.UrlHandle,
            AccentColor = request.AccentColor
        };

        var response = await repository.CreateCategoryAsync(category);

        return Ok(response);
    }

    // GET: https://localhost:7180/api/category?query=html&sortBy=name&sortDirection=desc
    [HttpGet]
    public async Task<IActionResult> GetCategory([FromQuery] string? query, [FromQuery] string? sortBy, [FromQuery] string? sortDirection, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var categories = context.Categories.AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query))
        {
            categories = categories.Where(x => x.Name.Contains(query));
        }

        // Sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                categories = isAsc ? categories.OrderBy(x => x.Name) : categories.OrderByDescending(x => x.Name);
            }
            else if (sortBy.Equals("URL", StringComparison.OrdinalIgnoreCase))
            {
                var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                categories = isAsc ? categories.OrderBy(x => x.UrlHandle) : categories.OrderByDescending(x => x.UrlHandle);
            }
        }
        else
        {
            // Default: alphabetical by name
            categories = categories.OrderBy(x => x.Name);
        }

        // Pagination
        var skipResults = ((pageNumber ?? 1) - 1) * (pageSize ?? 100);
        categories = categories.Skip(skipResults).Take(pageSize ?? 100);

        // Projection query to get article count without loading full BlogPost entities
        var response = await categories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                UrlHandle = c.UrlHandle,
                Description = c.Description,
                AccentColor = c.AccentColor,
                ArticleCount = c.BlogPosts.Count(bp => bp.IsVisible)
            })
            .ToListAsync();

        return Ok(response);
    }

    //Get: https:/localhost:7180/api/category/{id}
    [HttpGet]
    [Route("{id:Guid}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
    {
        var existingCategory = await repository.GetByIdAsync(id);

        if (existingCategory == null)
        {
            return NotFound();
        }

        //map domain model to Dto

        var response = new CategoryDto
        {
            Id = existingCategory.Id,
            Name = existingCategory.Name,
            UrlHandle = existingCategory.UrlHandle,
            Description = existingCategory.Description,
            AccentColor = existingCategory.AccentColor
        };

        return Ok(response);
    }

    // PUT: https:/localhost:7180/api/category/{id}
    [HttpPut]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, UpdateCategoryRequestDto request)
    {
        //convert Dto to domain model

        var category = new Category()
        {
            Id = id,
            Name = request.Name,
            UrlHandle = request.UrlHandle,
            AccentColor = request.AccentColor
        };
        category = await repository.UpdateAsync(category);

        if (category == null)
        {
            return NotFound();
        }
        // map domain model to Dto
        var response = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle,
            AccentColor = category.AccentColor
        };
        return Ok(response);

    }


    //DELETE: https:/localhost:7180/api/category/{id}
    [HttpDelete]
    [Route("{id:Guid}")]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var existingCategory = await repository.DeleteAsync(id);

        if (existingCategory == null)
        {
            return NotFound();
        }

        // map domain model to Dto
        var response = new CategoryDto
        {
            Id = existingCategory.Id,
            Name = existingCategory.Name,
            UrlHandle = existingCategory.UrlHandle,
            AccentColor = existingCategory.AccentColor
        };

        return Ok(response);
    }

    //GET: https://locahost:7226/api/categories/count
    [HttpGet]
    [Route("count")]
    // [Authorize(Roles = "Writer")]
    public async Task<IActionResult> GetCategoriesTotal()
    {
        var count = await repository.GetCount();

        return Ok(count);
        
    }

}