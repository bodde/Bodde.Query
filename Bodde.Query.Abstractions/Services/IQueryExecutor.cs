using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

/// <summary>
/// The service that executes queries and creates query results.
/// </summary>
public interface IQueryExecutor
{   
    /// <summary>
    /// Executes a query and returns its result.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query with criteria to execute.</param>
    /// <returns>The query result.</returns>
    QueryCriteriaResult<T> ToResult<T>(QueryWithCriteria<T> query);

    /// <summary>
    /// Asynchronously executes a query and returns its result.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query with criteria to execute.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the query result.</returns>
    Task<QueryCriteriaResult<T>> ToResultAsync<T>(
        QueryWithCriteria<T> query,
        CancellationToken cancellationToken = default
        );
}
