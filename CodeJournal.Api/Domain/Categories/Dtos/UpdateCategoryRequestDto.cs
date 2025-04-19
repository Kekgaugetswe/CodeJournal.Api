using System;

namespace CodeJournal.Api.Domain.Categories.Dtos;

public class UpdateCategoryRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string UrlHandle { get; set; } = string.Empty;

}
