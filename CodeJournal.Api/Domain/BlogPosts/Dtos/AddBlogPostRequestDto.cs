using System;

namespace CodeJournal.Api.Domain.BlogPosts.Dtos;

public class AddBlogPostRequestDto
{
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
}
