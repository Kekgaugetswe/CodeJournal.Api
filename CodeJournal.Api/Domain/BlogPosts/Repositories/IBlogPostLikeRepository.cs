using System;

namespace CodeJournal.Api.Domain.BlogPosts.Repositories;

public interface IBlogPostLikeRepository
{
    Task<int> GetTotalLikesForBlog(Guid BlogPostId);

}
