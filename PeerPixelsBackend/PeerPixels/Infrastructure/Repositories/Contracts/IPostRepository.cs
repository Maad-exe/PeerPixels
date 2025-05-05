using PeerPixels.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Repositories.Contracts
{
    /// <summary>
    /// Repository interface for managing post entities.
    /// Extends the base repository with post-specific operations.
    /// </summary>
    public interface IPostRepository : IBaseRepository<Post>
    {
        /// <summary>
        /// Retrieves a post by its ID with the associated user information.
        /// </summary>
        /// <param name="id">The unique identifier of the post to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the post with user details or null if not found.</returns>
        Task<Post?> GetPostWithUserAsync(int id);

        /// <summary>
        /// Retrieves all posts created by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose posts are being retrieved.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of the user's posts.</returns>
        Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId);

        /// <summary>
        /// Retrieves paginated feed posts for a specific user.
        /// Includes posts from users that the specified user is following.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom to retrieve the feed.</param>
        /// <param name="skip">The number of posts to skip for pagination.</param>
        /// <param name="take">The maximum number of posts to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of feed posts.</returns>
        Task<IEnumerable<Post>> GetFeedPostsAsync(string userId, int skip, int take);
    }
}