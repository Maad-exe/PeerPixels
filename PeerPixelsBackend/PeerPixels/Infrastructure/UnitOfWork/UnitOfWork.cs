using PeerPixels.Infrastructure.Data;
using PeerPixels.Infrastructure.Repositories.Contracts;
using PeerPixels.Infrastructure.UnitOfWork.Contracts;
using System;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Implementation of the Unit of Work pattern.
    /// Coordinates the work of multiple repositories in a single transaction context.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool _disposed;

        /// <summary>
        /// Gets the repository for user entities.
        /// </summary>
        public IUserRepository Users { get; }

        /// <summary>
        /// Gets the repository for post entities.
        /// </summary>
        public IPostRepository Posts { get; }

        /// <summary>
        /// Gets the repository for follow relationship entities.
        /// </summary>
        public IFollowRepository Follows { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="userRepository">The repository for user operations.</param>
        /// <param name="postRepository">The repository for post operations.</param>
        /// <param name="followRepository">The repository for follow relationship operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters is null.</exception>
        public UnitOfWork(
            ApplicationDbContext context,
            IUserRepository userRepository,
            IPostRepository postRepository,
            IFollowRepository followRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Users = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            Posts = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
            Follows = followRepository ?? throw new ArgumentNullException(nameof(followRepository));
            _disposed = false;
        }

        /// <summary>
        /// Completes the unit of work, saving all changes to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the number of affected entities.</returns>
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Disposes the resources used by the unit of work.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the resources used by the unit of work.
        /// </summary>
        /// <param name="disposing">Indicates whether the method is called from the Dispose method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}