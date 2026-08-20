using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Core;

/// <summary>
/// Provides extension methods for queries with criteria.
/// </summary>
public static class QueryWithCriteriaExtensions
{
    extension<T>(QueryWithCriteria<T> query)
    {
        /// <summary>
        /// Returns a query with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <param name="name">The name to assign to the query.</param>
        /// <returns>A query with the specified name.</returns>
        public QueryWithCriteria<T> WithName(string name)
        {     
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        
            return new(
                name,
                query.Toolkit,
                query.InputQuery,
                query.Criteria
            );
        }

        /// <summary>
        /// Parses and adds a filter to the query criteria.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <param name="filterStatement">The filter statement to parse and apply.</param>
        /// <returns>A query with the filter applied to its criteria.</returns>
        public QueryWithCriteria<T> WithFilter(string filterStatement)
        { 
            if (filterStatement == null)
                throw new ArgumentNullException(nameof(filterStatement));

            var filterExpression = query.Toolkit.Parser.ParseFilterExpression(filterStatement);

            return new QueryWithCriteria<T>(
                query.Name,
                query.Toolkit,
                query.InputQuery,
                query.Criteria.WithFilter(filterExpression)
            );
        }

        /// <summary>
        /// Parses and adds ordering to the query criteria.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <param name="orderByStatement">The ordering statement to parse and apply.</param>
        /// <returns>A query with the ordering applied to its criteria.</returns>
        public QueryWithCriteria<T> WithOrderBy(string orderByStatement)
        {
            if (orderByStatement == null)
                throw new ArgumentNullException(nameof(orderByStatement));

            var orderByCriteria = query.Toolkit.Parser.ParseOrderBy(orderByStatement);

            return new(
                query.Name,
                query.Toolkit,
                query.InputQuery,
                query.Criteria.WithOrderBy(orderByCriteria)
            );
        }

        /// <summary>
        /// Adds paging criteria to the query.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="top">The maximum number of items to return.</param>
        /// <param name="totalCount">Whether to include the total count.</param>
        /// <returns>A query with the paging criteria applied.</returns>
        public QueryWithCriteria<T> WithPaging(int? skip = null, int? top = null, bool? totalCount = null)
            => new(
                query.Name,
                query.Toolkit,
                query.InputQuery,
                query.Criteria.WithPaging(new(skip, top, totalCount))
            );

        /// <summary>
        /// Gets whether the query requests the total count.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <returns><see langword="true"/> when the query requests the total count; otherwise, <see langword="false"/>.</returns>
        public bool RequiresTotalCount() => query.Criteria.Paging?.TotalCount == true;

        /// <summary>
        /// Creates a query intended to retrieve the total count.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <returns>A query configured for retrieving the total count.</returns>
        public QueryWithCriteria<T> ForTotalCount()
        {
            var (name, toolkit, inputQuery, criteria) = query;
            var queryForCountName = $"{name} Count";

            return inputQuery.WithCriteria(queryForCountName, criteria.ForTotalCount(), toolkit);
        }

        /// <summary>
        /// Executes the query synchronously.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <returns>The query result.</returns>
        public QueryCriteriaResult<T> ToResult()
            => query.Toolkit.Executor.ToResult(query);


        /// <summary>
        /// Executes the query asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the query elements.</typeparam>
        /// <returns>A task that represents the asynchronous operation and contains the query result.</returns>
        public Task<QueryCriteriaResult<T>> ToResultAsync()
            => query.Toolkit.Executor.ToResultAsync(query);
    }
}