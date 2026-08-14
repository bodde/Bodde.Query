using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

public static class QueryableWithCriteriaExtensions
{
    extension<T>(QueryableWithCriteria<T> query)
    {
        public QueryableWithCriteria<T> Filter(string filterStatement)
        {
            var filterExpression = query.Toolkit.Parser.ParseFilterExpression(filterStatement);
            if (filterExpression is null)
                return query;

            filterExpression = query.Criteria.Filter == null 
                ? filterExpression
                : new FilterCriteria.LogicalExpression(FilterCriteria.LogicalOperator.And, query.Criteria.Filter.Expression, filterExpression);

            var newCriteria = new QueryCriteria(
                Filter: new FilterCriteria(filterExpression),
                OrderBy: query.Criteria.OrderBy,
                Paging: query.Criteria.Paging
            );

            return new QueryableWithCriteria<T>(
                query.Toolkit,
                query.Queryable,
                newCriteria
            );
        }
    
        public QueryCriteriaResult<T> ToResult()
            => query.Toolkit.Handler.ToResult(query);
    }
}