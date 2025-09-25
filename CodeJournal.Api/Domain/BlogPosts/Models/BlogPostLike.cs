using System;

namespace CodeJournal.Api.Domain.BlogPosts.Models;

public class BlogPostLike
{
    public Guid Id { get; set; }
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }

}
