using System;
using CodeJournal.Api.Domain.BlogPosts.Dtos;
using CodeJournal.Api.Domain.BlogPosts.Models;

namespace CodeJournal.Api.Domain.BlogPosts.Repositories;

public interface IBlogPostLikeRepository
{
    Task<int> GetTotalLikesForBlog(Guid BlogPostId);
    Task AddLikeForBlog(AddBlogPostRequestDto request);

}
