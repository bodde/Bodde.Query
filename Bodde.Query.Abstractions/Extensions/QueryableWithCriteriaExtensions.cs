using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Extensions;

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
            ArgumentNullException.ThrowIfNull(filterStatement, nameof(filterStatement));

            var filterExpression = query.Toolkit.Parser.ParseFilterExpression(filterStatement);

            return new QueryableWithCriteria<T>(
                query.Name,
                query.Toolkit,
                query.InputQuery,
                query.Criteria.WithFilter(filterExpression)
            );
        }

        public QueryableWithCriteria<T> WithOrderBy(string orderByStatement)
        {
            ArgumentNullException.ThrowIfNull(orderByStatement, nameof(orderByStatement));

            var orderByCriteria = query.Toolkit.Parser.ParseOrderBy(orderByStatement);

            return new(
                query.Name,
                query.Toolkit,
                query.InputQuery,
                query.Criteria.WithOrderBy(orderByCriteria)
            );
        }

        public QueryableWithCriteria<T> WithPaging(int? skip = null, int? top = null, bool? totalCount = null)
            => new(
                query.Name,
                query.Toolkit,
                query.InputQuery,
                query.Criteria.WithPaging(new(skip, top, totalCount))
            );

        public bool RequiresTotalCount() => query.Criteria.Paging?.TotalCount == true;

        public QueryableWithCriteria<T> ForTotalCount()
        {
            var (name, toolkit, inputQuery, criteria) = query;
            var queryForCountName = $"{name} Count";

            return inputQuery.WithCriteria(queryForCountName, criteria.ForTotalCount(), toolkit);
        }

        public QueryCriteriaResult<T> ToResult()
            => query.Toolkit.Executor.ToResult(query);


        public Task<QueryCriteriaResult<T>> ToResultAsync()
            => query.Toolkit.Executor.ToResultAsync(query);
    }
}