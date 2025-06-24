using System;

namespace CodeJournal.Api.Domain.BlogPosts.Models;

public class UploadImageModel
{
    public IFormFile File { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

}