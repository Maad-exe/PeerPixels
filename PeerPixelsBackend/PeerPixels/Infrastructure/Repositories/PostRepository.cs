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
    /// Repository implementation for managing post entities.
    /// Provides methods for querying and manipulating post data.
    /// </summary>
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public PostRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a post by its ID with the associated user information.
        /// </summary>
        /// <param name="id">The unique identifier of the post to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the post with user details or null if not found.</returns>
        public async Task<Post?> GetPostWithUserAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Retrieves all posts created by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose posts are being retrieved.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of the user's posts.</returns>
        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Post>();

            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves paginated feed posts for a specific user.
        /// Includes posts from users that the specified user is following.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom to retrieve the feed.</param>
        /// <param name="skip">The number of posts to skip for pagination.</param>
        /// <param name="take">The maximum number of posts to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of feed posts.</returns>
        public async Task<IEnumerable<Post>> GetFeedPostsAsync(string userId, int skip, int take)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Post>();

            // Get IDs of users that the current user follows
            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FolloweeId)
                .ToListAsync();

            // Handle case where user doesn't follow anyone
            if (followingIds.Count == 0)
                return new List<Post>();

            // Get posts from those users
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => followingIds.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}