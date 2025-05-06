using System;
using CodeJournal.Api.Domain.BlogPosts.Models;

namespace CodeJournal.Api.Domain.BlogPosts.Repositories;

public interface IImageRepository
{
    Task<BlogImage> Upload(IFormFile file, BlogImage blogImage);


}
