using Microsoft.EntityFrameworkCore;
using PeerPixels.Core.Entities;
using PeerPixels.Infrastructure.Data;
using PeerPixels.Infrastructure.Repositories.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing user entities.
    /// Provides methods for querying and manipulating user data.
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the user or null if not found.</returns>
        public async Task<User?> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the user or null if not found.</returns>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        /// <summary>
        /// Retrieves a user with detailed information including associated posts and relationships.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve with details.</param>
        /// <returns>A task representing the asynchronous operation, containing the detailed user info or null if not found.</returns>
        public async Task<User?> GetUserWithDetailsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return await _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Retrieves all users who follow a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose followers are being retrieved.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of followers.</returns>
        public async Task<IEnumerable<User>> GetUserFollowersAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<User>();

            return await _context.Follows
                .Where(f => f.FolloweeId == userId)
                .Select(f => f.Follower)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all users that a specific user is following.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose following list is being retrieved.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of followed users.</returns>
        public async Task<IEnumerable<User>> GetUserFollowingAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<User>();

            return await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.Followee)
                .ToListAsync();
        }

        /// <summary>
        /// Determines whether one user is following another user.
        /// </summary>
        /// <param name="followerId">The unique identifier of the potential follower.</param>
        /// <param name="followeeId">The unique identifier of the potentially followed user.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean indicating whether the follow relationship exists.</returns>
        public async Task<bool> IsFollowingAsync(string followerId, string followeeId)
        {
            if (string.IsNullOrEmpty(followerId) || string.IsNullOrEmpty(followeeId))
                return false;

            return await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
        }
    }
}