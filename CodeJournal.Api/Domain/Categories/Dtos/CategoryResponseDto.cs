using System;

namespace CodeJournal.Api.Domain.Categories.Dtos;

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UrlHandle { get; set; }

}
