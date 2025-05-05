using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PeerPixels.Infrastructure.Repositories.Contracts
{
    /// <summary>
    /// Generic repository interface providing basic CRUD operations for entity types.
    /// </summary>
    /// <typeparam name="T">The entity type for which this repository provides operations.</typeparam>
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, containing the retrieved entity or null if not found.</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves all entities of type T.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing a collection of all entities.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Finds entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of matching entities.</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Adds multiple entities to the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(T entity);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(T entity);

        /// <summary>
        /// Removes multiple entities from the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to remove.</param>
        void RemoveRange(IEnumerable<T> entities);
    }
}