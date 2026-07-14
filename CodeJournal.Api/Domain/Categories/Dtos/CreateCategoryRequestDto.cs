namespace CodeJournal.Api.Domain.Categories.Dtos;

public class CreateCategoryRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string UrlHandle { get; set; } = string.Empty;
    public string? AccentColor { get; set; }
}
