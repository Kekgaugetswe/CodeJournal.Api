using System;

namespace CodeJournal.Api.Domain.BlogPosts.Dtos;

public class BlogPostCommentDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } 
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset DateAdded { get; set; }

}
