namespace CodeJournal.Api.Domain.BlogPosts.Dtos;

public class BlogPostFilterParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Title { get; set; }
    public string? Author { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsVisible { get; set; }
}
