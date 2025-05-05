using PeerPixels.Core.Entities;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Repositories.Contracts
{
    /// <summary>
    /// Repository interface for managing follow relationships between users.
    /// Extends the base repository with follow-specific operations.
    /// </summary>
    public interface IFollowRepository : IBaseRepository<Follow>
    {
        /// <summary>
        /// Retrieves a specific follow relationship between two users.
        /// </summary>
        /// <param name="followerId">The unique identifier of the following user.</param>
        /// <param name="followeeId">The unique identifier of the followed user.</param>
        /// <returns>A task representing the asynchronous operation, containing the follow relationship or null if not found.</returns>
        Task<Follow?> GetFollowAsync(string followerId, string followeeId);

        /// <summary>
        /// Counts the number of followers for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, containing the count of followers.</returns>
        Task<int> GetFollowersCountAsync(string userId);

        /// <summary>
        /// Counts the number of users that a specific user is following.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, containing the count of followed users.</returns>
        Task<int> GetFollowingCountAsync(string userId);
    }
}