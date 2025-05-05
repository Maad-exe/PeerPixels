using PeerPixels.Infrastructure.Repositories.Contracts;
using System;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.UnitOfWork.Contracts
{
    /// <summary>
    /// Interface for the Unit of Work pattern implementation.
    /// Coordinates the work of multiple repositories in a single transaction.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository for user entities.
        /// </summary>
        IUserRepository Users { get; }

        /// <summary>
        /// Gets the repository for post entities.
        /// </summary>
        IPostRepository Posts { get; }

        /// <summary>
        /// Gets the repository for follow relationship entities.
        /// </summary>
        IFollowRepository Follows { get; }

        /// <summary>
        /// Completes the unit of work, saving all changes to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the number of affected entities.</returns>
        Task<int> CompleteAsync();
    }
}