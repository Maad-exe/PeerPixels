
using PeerPixels.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Services.Contracts  
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(string id, string currentUserId);
        Task<UserDto> GetUserByUsernameAsync(string username, string currentUserId);
        Task<UserDto> UpdateUserAsync(string id, UpdateUserDto updateUserDto);
        Task<IEnumerable<UserDto>> GetUserFollowersAsync(string userId, string currentUserId);
        Task<IEnumerable<UserDto>> GetUserFollowingAsync(string userId, string currentUserId);
    }
}