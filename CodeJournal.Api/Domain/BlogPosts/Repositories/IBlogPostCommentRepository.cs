using System;
using CodeJournal.Api.Domain.BlogPosts.Models;

namespace CodeJournal.Api.Domain.BlogPosts.Repositories;

public interface IBlogPostCommentRepository
{
    Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment);
    Task<IEnumerable<BlogPostComment>> GetAllAsync(Guid blogPostId);
    Task<BlogPostComment?> GetByIdAsync(Guid id);
}
