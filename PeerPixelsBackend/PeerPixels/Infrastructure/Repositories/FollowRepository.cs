using Microsoft.EntityFrameworkCore;
using PeerPixels.Core.Entities;
using PeerPixels.Infrastructure.Data;
using PeerPixels.Infrastructure.Repositories.Contracts;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing follow relationships between users.
    /// Provides methods for querying and manipulating follow data.
    /// </summary>
    public class FollowRepository : BaseRepository<Follow>, IFollowRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FollowRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public FollowRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a specific follow relationship between two users.
        /// </summary>
        /// <param name="followerId">The unique identifier of the following user.</param>
        /// <param name="followeeId">The unique identifier of the followed user.</param>
        /// <returns>A task representing the asynchronous operation, containing the follow relationship or null if not found.</returns>
        public async Task<Follow?> GetFollowAsync(string followerId, string followeeId)
        {
            if (string.IsNullOrEmpty(followerId) || string.IsNullOrEmpty(followeeId))
                return null;

            return await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
        }

        /// <summary>
        /// Counts the number of followers for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, containing the count of followers.</returns>
        public async Task<int> GetFollowersCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return 0;

            return await _context.Follows
                .CountAsync(f => f.FolloweeId == userId);
        }

        /// <summary>
        /// Counts the number of users that a specific user is following.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task representing the asynchronous operation, containing the count of followed users.</returns>
        public async Task<int> GetFollowingCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return 0;

            return await _context.Follows
                .CountAsync(f => f.FollowerId == userId);
        }
    }
}