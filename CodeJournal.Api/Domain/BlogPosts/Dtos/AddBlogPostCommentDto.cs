using System;

namespace CodeJournal.Api.Domain.BlogPosts.Dtos;

public class AddBlogPostCommentDto
{
    public string Description { get; set; } = string.Empty;
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ParentCommentId { get; set; }
}
