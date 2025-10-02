using System;

namespace CodeJournal.Api.Domain.BlogPosts.Models;

public class AddBlogPostLike
{
    public Guid BlogPostId { get; set; }
    public Guid UserId { get; set; }
    

}
