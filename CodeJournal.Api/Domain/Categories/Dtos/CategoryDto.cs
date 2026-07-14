namespace CodeJournal.Api.Domain.Categories.Dtos;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UrlHandle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? AccentColor { get; set; }
    public int ArticleCount { get; set; }
}
