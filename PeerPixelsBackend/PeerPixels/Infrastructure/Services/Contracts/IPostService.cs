
using PeerPixels.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Services.Contracts
{
    public interface IPostService
    {
        Task<PostDto> CreatePostAsync(string userId, CreatePostDto createPostDto);
        Task<PostDto> GetPostByIdAsync(int id);
        Task<IEnumerable<PostDto>> GetPostsByUserIdAsync(string userId);
        Task<IEnumerable<PostDto>> GetFeedPostsAsync(string userId, int page, int pageSize);
    }
}