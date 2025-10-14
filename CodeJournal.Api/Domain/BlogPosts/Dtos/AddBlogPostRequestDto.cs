using System;

namespace CodeJournal.Api.Domain.BlogPosts.Dtos;

public class AddBlogPostRequestDto
{
    public Guid BlogPostId { get; set; }
    public string UserId { get; set; } = string.Empty;
}
