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

         var response = new List<CategoryDto>();

         foreach( var category in categories){

            response.Add(new CategoryDto{
                Id = category.Id,   
                Name = category.Name,
                UrlHandle = category.UrlHandle
            });

         }

         return Ok(response);

    }

    //Get: https:/localhost:7180/api/category/{id}
    [HttpGet]
    [Route("{id:Guid}")]

    public async Task<IActionResult> GetCategoryById([FromRoute]Guid id)
    {
        var existingCategory = await repository.GetById(id);

        if (existingCategory == null)
        {
            return NotFound();
        }

        //map domain model to Dto

        var response = new CategoryDto
        {
            Id = existingCategory.Id,
            Name = existingCategory.Name,
            UrlHandle = existingCategory.UrlHandle
        };

        return Ok(response);
    }

// PUT: https:/localhost:7180/api/category/{id}
    [HttpPut] 
    [Route("{id:Guid}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id,  UpdateCategoryRequestDto request)
    {
        //convert Dto to domain model

        var category = new Category()
        {
            Id = id,
            Name = request.Name,
            UrlHandle = request.UrlHandle
        };
        category = await repository.UpdateAsync(category);

        if(category == null)
        {
            return NotFound();
        }
        // map domain model to Dto
        var response = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            UrlHandle = category.UrlHandle
        };
        return Ok(response);
       
    }
   

}