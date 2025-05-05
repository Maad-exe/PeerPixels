using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeerPixels.DTOs;
using PeerPixels.Infrastructure.Services.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PeerPixels.Controllers
{
    /// <summary>
    /// Controller responsible for managing post-related operations
    /// including creating, retrieving, and managing feed posts.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostsController"/> class.
        /// </summary>
        /// <param name="postService">The post service for handling post operations.</param>
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// Retrieves a specific post by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the post to retrieve.</param>
        /// <returns>An action result containing the requested post.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
                return NotFound();
            return Ok(post);
        }

        /// <summary>
        /// Retrieves all posts created by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose posts are being retrieved.</param>
        /// <returns>An action result containing the collection of posts.</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPostsByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID cannot be null or empty");

            var posts = await _postService.GetPostsByUserIdAsync(userId);
            return Ok(posts);
        }

        /// <summary>
        /// Retrieves posts for the authenticated user's feed with pagination support.
        /// </summary>
        /// <param name="page">The page number of results to retrieve (defaults to 1).</param>
        /// <param name="pageSize">The number of posts per page (defaults to 10).</param>
        /// <returns>An action result containing the paginated feed posts.</returns>
        [Authorize]
        [HttpGet("feed")]
        public async Task<IActionResult> GetFeedPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User is not properly authenticated");

            var posts = await _postService.GetFeedPostsAsync(userId, page, pageSize);
            return Ok(posts);
        }

        /// <summary>
        /// Creates a new post for the authenticated user.
        /// </summary>
        /// <param name="createPostDto">The data transfer object containing post creation details.</param>
        /// <returns>An action result containing the newly created post.</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostDto createPostDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User is not properly authenticated");

            var post = await _postService.CreatePostAsync(userId, createPostDto);
            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
        }
    }
}