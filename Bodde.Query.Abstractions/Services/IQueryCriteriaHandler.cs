using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

/// <summary>
/// The service that applies query criteria to an <see cref="IQueryable{T}"/>.
/// </summary>
public interface IQueryCriteriaHandler
{
    /// <summary>
    /// Applies filtering, ordering, and paging criteria to a query.
    /// </summary>
    /// <typeparam name="T">The type of the query elements.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="criteria">The criteria to apply.</param>
    /// <returns>The query with the specified criteria applied.</returns>
    IQueryable<T> ApplyCriteria<T>(IQueryable<T> query, QueryCriteria criteria);
}
