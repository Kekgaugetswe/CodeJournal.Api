using System;

namespace CodeJournal.Api.Domain.Categories.Dtos;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UrlHandle { get; set; }

}
