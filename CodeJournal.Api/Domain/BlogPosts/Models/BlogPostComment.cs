using System;

namespace CodeJournal.Api.Domain.BlogPosts.Models;

public class BlogPostComment
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset DateAdded { get; set; }
    
    // Reply support
    public Guid? ParentCommentId { get; set; }
    public BlogPostComment? ParentComment { get; set; }
    public ICollection<BlogPostComment> Replies { get; set; } = new List<BlogPostComment>();

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }

    // Likes
    public ICollection<CommentLike> Likes { get; set; } = new List<CommentLike>();
}
