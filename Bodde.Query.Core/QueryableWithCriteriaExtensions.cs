using System.ComponentModel;
using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Core;

public static class QueryableWithCriteriaExtensions
{
    extension<T>(QueryableWithCriteria<T> query)
    {
        public QueryableWithCriteria<T> WithName(string name)
        {     
            ArgumentNullException.ThrowIfNull(name);
        
            return new(
                name,
                query.Toolkit,
                query.InputQuery,
                query.Criteria
            );
        }

        public QueryableWithCriteria<T> WithFilter(string filterStatement)
        {
            ArgumentNullException.ThrowIfNull(filterStatement);

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
                query.Name,
                query.Toolkit,
                query.InputQuery,
                newCriteria
            );
        }

        public QueryableWithCriteria<T> WithOrderBy(string orderByStatement)
        {
            ArgumentNullException.ThrowIfNull(orderByStatement);

            var orderByCriteria = query.Toolkit.Parser.ParseOrderBy(orderByStatement);

            var newCriteria = new QueryCriteria(
                Filter: query.Criteria.Filter,
                OrderBy: orderByCriteria,
                Paging: query.Criteria.Paging
            );

            return new QueryableWithCriteria<T>(
                query.Name,
                query.Toolkit,
                query.InputQuery,
                newCriteria
            );
        }

        public QueryableWithCriteria<T> WithPaging(int skip, int top, bool totalCount = true)
        {
            var newCriteria = new QueryCriteria(
                Filter: query.Criteria.Filter,
                OrderBy: query.Criteria.OrderBy,
                Paging: new PagingCriteria(skip, top, totalCount)
            );

            return new QueryableWithCriteria<T>(
                query.Name,
                query.Toolkit,
                query.InputQuery,
                newCriteria
            );
        }

        public bool RequiresTotalCount() => query.Criteria.Paging?.TotalCount == true;

        public QueryableWithCriteria<T> ForTotalCount()
        {
            var (name, toolkit, queryable, criteria) = query;
            var criteriaForCount = new QueryCriteria(Filter: criteria.Filter);
            var queryForCountName = $"{name} Count";

            return queryable.WithCriteria(queryForCountName, criteriaForCount, toolkit);
        }

        public QueryCriteriaResult<T> ToResult()
            => query.Toolkit.Executor.ToResult(query);


        public Task<QueryCriteriaResult<T>> ToResultAsync()
            => query.Toolkit.Executor.ToResultAsync(query);
    }
}