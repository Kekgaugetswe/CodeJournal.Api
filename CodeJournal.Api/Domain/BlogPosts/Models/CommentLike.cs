namespace CodeJournal.Api.Domain.BlogPosts.Models;

public class CommentLike
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public BlogPostComment Comment { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
