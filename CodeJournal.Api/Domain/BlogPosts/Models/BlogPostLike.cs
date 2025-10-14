using System;

namespace CodeJournal.Api.Domain.BlogPosts.Models;

public class BlogPostLike
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime LikedAt { get; set; }
    public BlogPost BlogPost { get; set; }



}
