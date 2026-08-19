using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Abstractions.Extensions;

/// <summary>
/// Provides extension methods for creating queries with criteria.
/// </summary>
public static class QueryableExtensions
{
    extension<T>(IQueryable<T> query)
    {
        /// <summary>
        /// Creates a query wrapper with empty criteria.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <param name="queryToolkit">The toolkit used to process the query.</param>
        /// <returns>A query wrapper with empty criteria.</returns>
        public QueryWithCriteria<T> WithCriteria(IQueryToolkit queryToolkit)
            => new(string.Empty, queryToolkit, query, new QueryCriteria());

        /// <summary>
        /// Creates a named query wrapper with empty criteria.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <param name="name">The name of the query.</param>
        /// <param name="queryToolkit">The toolkit used to process the query.</param>
        /// <returns>A named query wrapper with empty criteria.</returns>
        public QueryWithCriteria<T> WithCriteria(string name, IQueryToolkit queryToolkit)
            => new(name, queryToolkit, query, new QueryCriteria());

        /// <summary>
        /// Creates a query wrapper with the specified criteria.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <param name="criteria">The criteria associated with the query.</param>
        /// <param name="queryToolkit">The toolkit used to process the query.</param>
        /// <returns>A query wrapper containing the specified criteria.</returns>
        public QueryWithCriteria<T> WithCriteria(QueryCriteria criteria, IQueryToolkit queryToolkit)
            => new(string.Empty, queryToolkit, query, criteria);

        /// <summary>
        /// Creates a named query wrapper with the specified criteria.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <param name="name">The name of the query.</param>
        /// <param name="criteria">The criteria associated with the query.</param>
        /// <param name="queryToolkit">The toolkit used to process the query.</param>
        /// <returns>A named query wrapper containing the specified criteria.</returns>
        public QueryWithCriteria<T> WithCriteria(string name, QueryCriteria criteria, IQueryToolkit queryToolkit)
            => new(name, queryToolkit, query, criteria);
    }
}