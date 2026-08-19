using Bodde.Query.Core;
using Microsoft.EntityFrameworkCore;

namespace Bodde.Query.EntityFrameworkCore;

/// <summary>
/// Executes queries using Entity Framework Core query operators.
/// </summary>
public class EntityFrameworkCoreQueryExecutor : QueryExecutor
{
    /// <summary>
    /// Counts the elements returned by an Entity Framework Core query.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query to count.</param>
    /// <returns>The number of query elements.</returns>
    protected override int Count<T>(IQueryable<T> query) => query.Count();

    /// <summary>
    /// Asynchronously counts the elements returned by an Entity Framework Core query.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query to count.</param>
    /// <param name="ct">The token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the number of query elements.</returns>
    protected override Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default)  => query.CountAsync(ct);

    /// <summary>
    /// Materializes an Entity Framework Core query into an array.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query to materialize.</param>
    /// <returns>The materialized query elements.</returns>
    protected override T[] ToArray<T>(IQueryable<T> query) => query.ToArray();

    /// <summary>
    /// Asynchronously materializes an Entity Framework Core query into an array.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query to materialize.</param>
    /// <param name="ct">The token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the materialized query elements.</returns>
    protected override Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default) => query.ToArrayAsync(ct);
}