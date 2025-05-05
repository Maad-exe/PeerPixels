using AutoMapper;
using PeerPixels.DTOs;
using PeerPixels.Infrastructure.Services.Contracts;
using PeerPixels.Infrastructure.UnitOfWork.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Services
{
    /// <summary>
    /// Service responsible for managing user operations, including profile retrieval,
    /// updates, and follow relationship queries.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The AutoMapper instance for object mapping.</param>
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve.</param>
        /// <param name="currentUserId">The unique identifier of the current authenticated user.</param>
        /// <returns>A task representing the asynchronous operation, containing the user data or null if not found.</returns>
        public async Task<UserDto?> GetUserByIdAsync(string id, string currentUserId)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var user = await _unitOfWork.Users.GetUserWithDetailsAsync(id);
            if (user == null)
                return null;

            var followersCount = await _unitOfWork.Follows.GetFollowersCountAsync(id);
            var followingCount = await _unitOfWork.Follows.GetFollowingCountAsync(id);
            var postsCount = user.Posts?.Count ?? 0;
            var isFollowing = false;

            if (!string.IsNullOrEmpty(currentUserId))
            {
                isFollowing = await _unitOfWork.Users.IsFollowingAsync(currentUserId, id);
            }

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                DisplayName = user.DisplayName,
                Email = user.Email ?? string.Empty,
                AvatarUrl = user.AvatarUrl,
                FollowersCount = followersCount,
                FollowingCount = followingCount,
                PostsCount = postsCount,
                IsFollowing = isFollowing
            };
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <param name="currentUserId">The unique identifier of the current authenticated user.</param>
        /// <returns>A task representing the asynchronous operation, containing the user data or null if not found.</returns>
        public async Task<UserDto?> GetUserByUsernameAsync(string username, string currentUserId)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var user = await _unitOfWork.Users.GetUserByUsernameAsync(username);
            if (user == null)
                return null;

            return await GetUserByIdAsync(user.Id, currentUserId);
        }

        /// <summary>
        /// Updates a user's profile information.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="updateUserDto">The data transfer object containing updated user information.</param>
        /// <returns>A task representing the asynchronous operation, containing the updated user data or null if not found.</returns>
        public async Task<UserDto?> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            if (updateUserDto == null)
                throw new System.ArgumentNullException(nameof(updateUserDto));

            var user = await _unitOfWork.Users.GetUserByIdAsync(id);
            if (user == null)
                return null;

            // Only update properties that were provided
            if (!string.IsNullOrEmpty(updateUserDto.DisplayName))
                user.DisplayName = updateUserDto.DisplayName;

            if (!string.IsNullOrEmpty(updateUserDto.AvatarUrl))
                user.AvatarUrl = updateUserDto.AvatarUrl;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return await GetUserByIdAsync(id, id);
        }

        /// <summary>
        /// Retrieves the followers of a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose followers are being retrieved.</param>
        /// <param name="currentUserId">The unique identifier of the current authenticated user.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of followers.</returns>
        public async Task<IEnumerable<UserDto>> GetUserFollowersAsync(string userId, string currentUserId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<UserDto>();

            currentUserId ??= string.Empty;

            var followers = await _unitOfWork.Users.GetUserFollowersAsync(userId);
            var userDtos = new List<UserDto>();

            foreach (var follower in followers)
            {
                var userDto = await GetUserByIdAsync(follower.Id, currentUserId);
                if (userDto != null)
                {
                    userDtos.Add(userDto);
                }
            }

            return userDtos;
        }

        /// <summary>
        /// Retrieves the users that a specific user is following.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose following list is being retrieved.</param>
        /// <param name="currentUserId">The unique identifier of the current authenticated user.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of followed users.</returns>
        public async Task<IEnumerable<UserDto>> GetUserFollowingAsync(string userId, string currentUserId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<UserDto>();

            currentUserId ??= string.Empty;

            var following = await _unitOfWork.Users.GetUserFollowingAsync(userId);
            var userDtos = new List<UserDto>();

            foreach (var followee in following)
            {
                var userDto = await GetUserByIdAsync(followee.Id, currentUserId);
                if (userDto != null)
                {
                    userDtos.Add(userDto);
                }
            }

            return userDtos;
        }
    }
}