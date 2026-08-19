using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

/// <summary>
/// Abstract class that provides the common workflow for synchronously and asynchronously executing queries with criteria.
/// </summary>
public abstract class QueryExecutor : IQueryExecutor
{
    /// <summary>
    /// Materializes a query into an array.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query to materialize.</param>
    /// <returns>The materialized query elements.</returns>
    protected abstract T[] ToArray<T>(IQueryable<T> query);

    /// <summary>
    /// Counts the elements returned by a query.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query to count.</param>
    /// <returns>The number of query elements.</returns>
    protected abstract int Count<T>(IQueryable<T> query);

    /// <summary>
    /// Asynchronously materializes a query into an array.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query to materialize.</param>
    /// <param name="ct">The token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the materialized query elements.</returns>
    protected abstract Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously counts the elements returned by a query.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query to count.</param>
    /// <param name="ct">The token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the number of query elements.</returns>
    protected abstract Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default);

    /// <summary>
    /// Executes a query and creates a result containing its items and optional total count.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query with criteria to execute.</param>
    /// <returns>The query result.</returns>
    public QueryCriteriaResult<T> ToResult<T>(QueryWithCriteria<T> query)
    {       
        var result = ToArray(query.OutputQuery);
        int? totalCount = query.RequiresTotalCount() ? Count(query.ForTotalCount().OutputQuery) : null;
        var criteria = query.Toolkit.Formatter.Format(query.Criteria);

        return new QueryCriteriaResult<T>(query.Name, criteria, result, totalCount);
    }

    /// <summary>
    /// Asynchronously executes a query and creates a result containing its items and optional total count.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The query with criteria to execute.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the query result.</returns>
    public async Task<QueryCriteriaResult<T>> ToResultAsync<T>(QueryWithCriteria<T> query, CancellationToken cancellationToken = default)
    {        
        var result = await ToArrayAsync(query.OutputQuery, cancellationToken).ConfigureAwait(false);
        int? totalCount = query.RequiresTotalCount()
            ? await CountAsync(query.ForTotalCount().OutputQuery, cancellationToken).ConfigureAwait(false)
            : null;
            
        return new QueryCriteriaResult<T>(query.Name, query.FormattedCriteria, result, totalCount);
    }
    
}