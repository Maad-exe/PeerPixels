
using PeerPixels.DTOs;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Services.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> GoogleLoginAsync(GoogleAuthDto googleAuthDto);
    }
}