using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeerPixels.DTOs;
using PeerPixels.Infrastructure.Services.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PeerPixels.Controllers
{
    /// <summary>
    /// Controller responsible for user-related operations including 
    /// user profile management, follow relationships, and user queries.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFollowService _followService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userService">The user service for handling user operations.</param>
        /// <param name="followService">The follow service for handling follow relationships.</param>
        public UsersController(IUserService userService, IFollowService followService)
        {
            _userService = userService;
            _followService = followService;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve.</param>
        /// <returns>An action result containing the requested user information.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID cannot be null or empty");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserByIdAsync(id, currentUserId ?? string.Empty);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>An action result containing the requested user information.</returns>
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("Username cannot be null or empty");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserByUsernameAsync(username, currentUserId ?? string.Empty);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Retrieves the followers of a specific user.
        /// </summary>
        /// <param name="id">The unique identifier of the user whose followers are being retrieved.</param>
        /// <returns>An action result containing the collection of followers.</returns>
        [HttpGet("{id}/followers")]
        public async Task<IActionResult> GetUserFollowers(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID cannot be null or empty");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followers = await _userService.GetUserFollowersAsync(id, currentUserId ?? string.Empty);
            return Ok(followers);
        }

        /// <summary>
        /// Retrieves the users that a specific user is following.
        /// </summary>
        /// <param name="id">The unique identifier of the user whose following list is being retrieved.</param>
        /// <returns>An action result containing the collection of followed users.</returns>
        [HttpGet("{id}/following")]
        public async Task<IActionResult> GetUserFollowing(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID cannot be null or empty");

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var following = await _userService.GetUserFollowingAsync(id, currentUserId ?? string.Empty);
            return Ok(following);
        }

        /// <summary>
        /// Updates the authenticated user's profile information.
        /// </summary>
        /// <param name="updateUserDto">The data transfer object containing updated user information.</param>
        /// <returns>An action result containing the updated user information.</returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UpdateUserDto updateUserDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User is not properly authenticated");

            var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }

        /// <summary>
        /// Establishes a follow relationship between the authenticated user and another user.
        /// </summary>
        /// <param name="id">The unique identifier of the user to follow.</param>
        /// <returns>An action result indicating the outcome of the follow operation.</returns>
        [Authorize]
        [HttpPost("follow/{id}")]
        public async Task<IActionResult> FollowUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID to follow cannot be null or empty");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User is not properly authenticated");

            if (userId == id)
                return BadRequest("Users cannot follow themselves");

            var result = await _followService.FollowUserAsync(userId, id);
            if (!result)
                return BadRequest("Failed to follow user");

            return Ok();
        }

        /// <summary>
        /// Removes a follow relationship between the authenticated user and another user.
        /// </summary>
        /// <param name="id">The unique identifier of the user to unfollow.</param>
        /// <returns>An action result indicating the outcome of the unfollow operation.</returns>
        [Authorize]
        [HttpDelete("unfollow/{id}")]
        public async Task<IActionResult> UnfollowUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID to unfollow cannot be null or empty");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User is not properly authenticated");

            var result = await _followService.UnfollowUserAsync(userId, id);
            if (!result)
                return BadRequest("Failed to unfollow user");

            return Ok();
        }
    }
}