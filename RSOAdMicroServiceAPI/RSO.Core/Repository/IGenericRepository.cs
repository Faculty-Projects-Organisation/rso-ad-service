﻿using System.Linq.Expressions;

namespace AdServiceRSO.Repository;

/// <summary>
/// Handles the basic CRUD operations.
/// </summary>
/// <typeparam name="T">Type of database generated object by the EF Core Power tools.</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Gets the type by its ID property in the database.
    /// </summary>
    /// <param name="id">The ID of the type.</param>
    /// <returns>The database object type.</returns>
    public Task<T> GetByIdAsync(int id);

    /// <summary>
    /// Gets all the enumerators for a database type.
    /// </summary>
    /// <returns>All enumerators for a database type.</returns>
    public Task<List<T>> GetAllAsync();

    /// <summary>
    /// Gets many enumerators for a database type based on a condition.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="orderBy"></param>
    /// <param name="top"></param>
    /// <param name="skip"></param>
    /// <param name="includeProperties"></param>
    /// <returns></returns>
    public Task<List<T>> GetManyAsync(Expression<Func<T, bool>> filter = null,
                              Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                              int? top = null,
                              int? skip = null,
                              params string[] includeProperties);

    /// <summary>
    /// Inserts a new entity of a database type.
    /// </summary>
    /// <param name="entity">The entity to be inserted to the database.</param>
    /// <returns>The insterted entity with the ID.</returns>
    public Task<T> InsertAsync(T entity);

    /// <summary>
    /// Inserts a List of new entities of a database type.
    /// </summary>
    /// <param name="entities">The List of new entities to be inserted to the database.</param>
    public Task InsertAsync(List<T> entities);

    /// <summary>
    /// Updates a new entity of a database type.
    /// </summary>
    /// <param name="entity">The entity to be updated to the database.</param>
    public Task UpdateAsync(T entity);

    /// <summary>
    /// Deletes the type by its ID property in the database.
    /// </summary>
    /// <param name="id">The ID of the type.</param>
    //public Task DeleteAsync(int id);

    /// <summary>
    /// Deletes a new entity of a database type.
    /// </summary>
    /// <param name="entity">The entity to be deleted to the database.</param>
    //public Task Delete(T entity);

    /// <summary>
    /// Deletes multiple, but not all records in the database.
    /// </summary>
    /// <param name="entities">The entities the are going to be deleted.</param>
    //public Task DeleteMultipleAsync(IEnumerable<T> entities);

    /// <summary>
    /// Deletes all the records of a database type.
    /// </summary>
    //public Task DeleteAllAsync();
}
