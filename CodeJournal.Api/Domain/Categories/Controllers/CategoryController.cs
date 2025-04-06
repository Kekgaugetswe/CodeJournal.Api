using CodeJournal.Api.Domain.Categories;
using CodeJournal.Api.Domain.Categories.Dtos;
using CodeJournal.Api.Domain.Categories.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeJournal.Api.Domain.Categories.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryRepository repository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryRequestDto request)
    {
        // map dto to domain model 

        var category = new Category()
        {
            Name = request.Name,
            UrlHandle = request.UrlHandle
        };

        var response = await repository.CreateCategoryAsync(category);

        return Ok(response);
    }
    [HttpGet]
    public async Task<IActionResult> GetCategory()
    {
        var categories = await repository.GetAllAsync();

         //map domain model to Dto

         var response = new List<CategoryResponseDto>();

         foreach( var category in categories){

            response.Add(new CategoryResponseDto{
                Id = category.Id,   
                Name = category.Name,
                UrlHandle = category.UrlHandle
            });

         }

         return Ok(response);

    }

}