namespace CodeJournal.Api.Domain.BlogPosts.Dtos;

public class BlogCommentDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset DateAdded { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public int ReplyCount { get; set; }
    public List<BlogCommentDto> Replies { get; set; } = new();
    public bool IsDeleted { get; set; }
    public int LikeCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
    public bool CanDelete { get; set; }
}
