using System;

namespace CodeJournal.Api.Domain.BlogPosts.Dtos;

public class BlogCommentDto
{
    public string Description { get; set; }
    public DateTimeOffset DateAdded { get; set; }
    public string UserName { get; set; }

}
