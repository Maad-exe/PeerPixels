using PeerPixels.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Repositories.Contracts
{
    /// <summary>
    /// Repository interface for managing user entities.
    /// Extends the base repository with user-specific operations.
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the user or null if not found.</returns>
        Task<User?> GetUserByIdAsync(string id);

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the user or null if not found.</returns>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Retrieves a user with detailed information including associated posts and relationships.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve with details.</param>
        /// <returns>A task representing the asynchronous operation, containing the detailed user info or null if not found.</returns>
        Task<User?> GetUserWithDetailsAsync(string id);

        /// <summary>
        /// Retrieves all users who follow a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose followers are being retrieved.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of followers.</returns>
        Task<IEnumerable<User>> GetUserFollowersAsync(string userId);

        /// <summary>
        /// Retrieves all users that a specific user is following.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose following list is being retrieved.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of followed users.</returns>
        Task<IEnumerable<User>> GetUserFollowingAsync(string userId);

        /// <summary>
        /// Determines whether one user is following another user.
        /// </summary>
        /// <param name="followerId">The unique identifier of the potential follower.</param>
        /// <param name="followeeId">The unique identifier of the potentially followed user.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean indicating whether the follow relationship exists.</returns>
        Task<bool> IsFollowingAsync(string followerId, string followeeId);
    }
}