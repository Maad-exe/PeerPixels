using PeerPixels.Core.Entities;
using PeerPixels.DTOs;
using PeerPixels.Infrastructure.Services.Contracts;
using PeerPixels.Infrastructure.UnitOfWork.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Services
{
    /// <summary>
    /// Service responsible for managing posts, including creation, retrieval, and feed generation.
    /// </summary>
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a new post for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user creating the post.</param>
        /// <param name="createPostDto">The data transfer object containing post details.</param>
        /// <returns>A task representing the asynchronous operation, containing the created post data.</returns>
        public async Task<PostDto> CreatePostAsync(string userId, CreatePostDto createPostDto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new System.ArgumentException("User ID cannot be null or empty", nameof(userId));

            if (createPostDto == null)
                throw new System.ArgumentNullException(nameof(createPostDto));

            var post = new Post
            {
                UserId = userId,
                ImageUrl = createPostDto.ImageUrl,
                Caption = createPostDto.Caption
            };

            await _unitOfWork.Posts.AddAsync(post);
            await _unitOfWork.CompleteAsync();

            return await GetPostByIdAsync(post.Id);
        }

        /// <summary>
        /// Retrieves a post by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the post to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the post data or null if not found.</returns>
        public async Task<PostDto?> GetPostByIdAsync(int id)
        {
            var post = await _unitOfWork.Posts.GetPostWithUserAsync(id);
            if (post == null)
                return null;

            return MapPostToDto(post);
        }

        /// <summary>
        /// Retrieves all posts created by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose posts are being retrieved.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of the user's posts.</returns>
        public async Task<IEnumerable<PostDto>> GetPostsByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<PostDto>();

            var posts = await _unitOfWork.Posts.GetPostsByUserIdAsync(userId);
            return posts.Select(MapPostToDto);
        }

        /// <summary>
        /// Retrieves paginated feed posts for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom to retrieve the feed.</param>
        /// <param name="page">The page number of results to retrieve.</param>
        /// <param name="pageSize">The number of posts per page.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of feed posts.</returns>
        public async Task<IEnumerable<PostDto>> GetFeedPostsAsync(string userId, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<PostDto>();

            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            var skip = (page - 1) * pageSize;
            var posts = await _unitOfWork.Posts.GetFeedPostsAsync(userId, skip, pageSize);

            return posts.Select(MapPostToDto);
        }

        /// <summary>
        /// Maps a Post entity to a PostDto.
        /// </summary>
        /// <param name="post">The post entity to map.</param>
        /// <returns>The mapped post DTO.</returns>
        private static PostDto MapPostToDto(Post post)
        {
            if (post == null)
                throw new System.ArgumentNullException(nameof(post));

            if (post.User == null)
                throw new System.InvalidOperationException("Post user navigation property is null");

            return new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                UserName = post.User.UserName ?? string.Empty,
                DisplayName = post.User.DisplayName,
                UserAvatarUrl = post.User.AvatarUrl,
                ImageUrl = post.ImageUrl,
                Caption = post.Caption,
                CreatedAt = post.CreatedAt
            };
        }
    }
}