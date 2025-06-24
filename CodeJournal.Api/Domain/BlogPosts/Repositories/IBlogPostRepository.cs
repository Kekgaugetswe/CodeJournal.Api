using System;
using CodeJournal.Api.Domain.BlogPosts.Models;

namespace CodeJournal.Api.Domain.BlogPosts.Repositories;

public interface IBlogPostRepository
{
    Task<BlogPost> CreateAsync(BlogPost blogPost);
    Task<IEnumerable<BlogPost>> GetAllAsync();

    Task<BlogPost?> GetByIdAsync(Guid id);

    Task<BlogPost?> GetByUrlHandleAsync(string urlHandle);

    Task<BlogPost?> UdpateAsync(BlogPost blogPost);

    Task<BlogPost?> DeleteAsync(Guid id);
    


}
