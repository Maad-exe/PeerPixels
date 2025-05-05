using PeerPixels.Core.Entities;
using PeerPixels.Infrastructure.Services.Contracts;
using PeerPixels.Infrastructure.UnitOfWork.Contracts;
using System;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Services
{
    /// <summary>
    /// Service responsible for managing follow relationships between users.
    /// </summary>
    public class FollowService : IFollowService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="FollowService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        public FollowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Establishes a follow relationship between two users.
        /// </summary>
        /// <param name="followerId">The unique identifier of the user who wants to follow another user.</param>
        /// <param name="followeeId">The unique identifier of the user to be followed.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean indicating success.</returns>
        public async Task<bool> FollowUserAsync(string followerId, string followeeId)
        {
            if (string.IsNullOrEmpty(followerId))
                throw new ArgumentException("Follower ID cannot be null or empty", nameof(followerId));

            if (string.IsNullOrEmpty(followeeId))
                throw new ArgumentException("Followee ID cannot be null or empty", nameof(followeeId));

            // Can't follow yourself
            if (followerId == followeeId)
                return false;

            // Check if both users exist
            var follower = await _unitOfWork.Users.GetUserByIdAsync(followerId);
            var followee = await _unitOfWork.Users.GetUserByIdAsync(followeeId);

            if (follower == null || followee == null)
                return false;

            // Check if already following
            var existingFollow = await _unitOfWork.Follows.GetFollowAsync(followerId, followeeId);
            if (existingFollow != null)
                return false;

            // Create new follow
            var follow = new Follow
            {
                FollowerId = followerId,
                FolloweeId = followeeId
            };

            await _unitOfWork.Follows.AddAsync(follow);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        /// <summary>
        /// Removes a follow relationship between two users.
        /// </summary>
        /// <param name="followerId">The unique identifier of the following user.</param>
        /// <param name="followeeId">The unique identifier of the followed user.</param>
        /// <returns>A task representing the asynchronous operation, containing a boolean indicating success.</returns>
        public async Task<bool> UnfollowUserAsync(string followerId, string followeeId)
        {
            if (string.IsNullOrEmpty(followerId))
                throw new ArgumentException("Follower ID cannot be null or empty", nameof(followerId));

            if (string.IsNullOrEmpty(followeeId))
                throw new ArgumentException("Followee ID cannot be null or empty", nameof(followeeId));

            var follow = await _unitOfWork.Follows.GetFollowAsync(followerId, followeeId);
            if (follow == null)
                return false;

            _unitOfWork.Follows.Remove(follow);
            await _unitOfWork.CompleteAsync();
            return true;
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

            return await _unitOfWork.Users.IsFollowingAsync(followerId, followeeId);
        }
    }
}