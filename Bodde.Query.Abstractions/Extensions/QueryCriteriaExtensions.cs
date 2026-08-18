using Bodde.Query.Abstractions.Models;
using static Bodde.Query.Abstractions.Models.FilterCriteria;

namespace Bodde.Query.Abstractions.Extensions;

public static class QueryCriteriaExtensions
{
    extension(QueryCriteria queryCriteria)
    {
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

        public QueryCriteria WithOrderBy(OrderByCriteria orderByCriteria)
            => new(
                Filter: queryCriteria.Filter,
                OrderBy: orderByCriteria,
                Paging: queryCriteria.Paging
            );

        public QueryCriteria WithPaging(PagingCriteria pagingCriteria)
            => new(
                Filter: queryCriteria.Filter,
                OrderBy: queryCriteria.OrderBy,
                Paging: pagingCriteria
            );

        
        public QueryCriteria ForTotalCount()
            => new(Filter: queryCriteria.Filter);
    }
}