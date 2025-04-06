using System;

namespace CodeJournal.Api.Domain.Categories.Dtos;

public class CreateCategoryRequestDto
{
    public string Name { get; set; }
    public string UrlHandle { get; set; }


}
