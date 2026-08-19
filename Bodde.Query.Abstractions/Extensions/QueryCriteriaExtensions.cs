using Bodde.Query.Abstractions.Models;
using static Bodde.Query.Abstractions.Models.FilterCriteria;

namespace Bodde.Query.Abstractions.Extensions;

/// <summary>
/// Provides extension methods for composing query criteria.
/// </summary>
public static class QueryCriteriaExtensions
{
    extension(QueryCriteria queryCriteria)
    {
        /// <summary>
        /// Adds a filter expression to the criteria, combining it with an existing filter using a logical AND.
        /// </summary>
        /// <param name="filterExpression">The filter expression to add.</param>
        /// <returns>New criteria containing the added filter expression.</returns>
        public QueryCriteria WithFilter(FilterExpression filterExpression)
        {
            var combinedFilterExpression = queryCriteria.Filter == null 
                ? filterExpression
                : new LogicalExpression(LogicalOperator.And, queryCriteria.Filter.Expression, filterExpression);

            return new(
                Filter: new(combinedFilterExpression),
                OrderBy: queryCriteria.OrderBy,
                Paging: queryCriteria.Paging
            );
        }

        /// <summary>
        /// Replaces the ordering criteria.
        /// </summary>
        /// <param name="orderByCriteria">The ordering criteria to set.</param>
        /// <returns>New criteria containing the specified ordering.</returns>
        public QueryCriteria WithOrderBy(OrderByCriteria orderByCriteria)
            => new(
                Filter: queryCriteria.Filter,
                OrderBy: orderByCriteria,
                Paging: queryCriteria.Paging
            );

        /// <summary>
        /// Replaces the paging criteria.
        /// </summary>
        /// <param name="pagingCriteria">The paging criteria to set.</param>
        /// <returns>New criteria containing the specified paging.</returns>
        public QueryCriteria WithPaging(PagingCriteria pagingCriteria)
            => new(
                Filter: queryCriteria.Filter,
                OrderBy: queryCriteria.OrderBy,
                Paging: pagingCriteria
            );

        /// <summary>
        /// Creates criteria containing only the filter, for use when retrieving a total count.
        /// </summary>
        /// <returns>New criteria containing the current filter only.</returns>
        public QueryCriteria ForTotalCount()
            => new(Filter: queryCriteria.Filter);
    }
}