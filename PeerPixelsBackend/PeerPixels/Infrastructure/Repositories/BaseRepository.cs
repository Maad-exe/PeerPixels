using Microsoft.EntityFrameworkCore;
using PeerPixels.Infrastructure.Data;
using PeerPixels.Infrastructure.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository base class implementing common data access operations.
    /// Provides a foundation for entity-specific repositories.
    /// </summary>
    /// <typeparam name="T">The entity type for which this repository provides operations.</typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        /// <summary>
        /// The database context used for data access operations.
        /// </summary>
        protected readonly ApplicationDbContext _context;

        /// <summary>
        /// The entity set for the specified entity type.
        /// </summary>
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the retrieved entity or null if not found.</returns>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Retrieves all entities of type T.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a collection of all entities.</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Finds entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of matching entities.</returns>
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Adds multiple entities to the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Removes multiple entities from the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to remove.</param>
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}