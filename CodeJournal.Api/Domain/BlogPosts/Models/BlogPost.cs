using System;
using CodeJournal.Api.Domain.Categories;

namespace CodeJournal.Api.Domain.BlogPosts.Models;

public class BlogPost
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

    //navigation properties
    public ICollection<Category> Categories { get; set; }
    public ICollection<BlogPostLike> Likes { get; set; }
    public ICollection<BlogPostComment> Comments { get; set; }

}