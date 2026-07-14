using CodeJournal.Api.Domain.BlogPosts.Models;

namespace CodeJournal.Api.Domain.Categories;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UrlHandle { get; set; }
    public string? Description { get; set; }
    public string? AccentColor { get; set; }
    public ICollection<BlogPost> BlogPosts { get; set; }
}
