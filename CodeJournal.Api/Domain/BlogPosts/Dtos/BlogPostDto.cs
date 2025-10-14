using System;
using CodeJournal.Api.Domain.Categories.Dtos;

namespace CodeJournal.Api.Domain.BlogPosts.Dtos;

public class BlogPostDto
{

    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FeaturedImageUrl { get; set; } = string.Empty;
    public string UrlHandle { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public string Author { get; set; } = string.Empty;
    public bool IsVisible { get; set; }
    public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    public int TotalLikes { get; set; }

    public bool Liked { get; set; }

}
