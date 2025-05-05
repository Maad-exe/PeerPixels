using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Services.Contracts
{
    public interface IFollowService
    {
        Task<bool> FollowUserAsync(string followerId, string followeeId);
        Task<bool> UnfollowUserAsync(string followerId, string followeeId);
        Task<bool> IsFollowingAsync(string followerId, string followeeId);
    }
}