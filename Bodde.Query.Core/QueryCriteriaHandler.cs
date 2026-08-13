using System.Linq.Expressions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

internal class QueryCriteriaHandler(
    IExpressionBuilder expressionBuilder, 
    IQueryExecutor queryExecutor
    ) : IQueryCriteriaHandler
{
    public QueryCriteriaQueryable<T> ApplyCriteria<T>(IQueryable<T> originalQuery, QueryCriteria criteria)
    {
        var queryWithCriteria = criteria.Filter != null 
            ? ApplyFilterCriteria(originalQuery, criteria.Filter) 
            : originalQuery;

        queryWithCriteria = criteria.OrderBy != null 
            ? ApplyOrderByCriteria(queryWithCriteria, criteria.OrderBy) 
            : queryWithCriteria;

        queryWithCriteria = criteria.Paging != null 
            ? ApplyPageCriteria(queryWithCriteria, criteria.Paging) 
            : queryWithCriteria;

        return new QueryCriteriaQueryable<T>(originalQuery, criteria, queryWithCriteria);
    }

    public async Task<QueryCriteriaResult<T>> ToResultAsync<T>(
        QueryCriteriaQueryable<T> query,
        CancellationToken cancellationToken = default
    )
    {
        var result = await queryExecutor.ToArrayAsync(query, cancellationToken);
        var requiresTotalCount = query.QueryCriteria.Paging?.TotalCount == true;
        if(!requiresTotalCount)
        {
            return new QueryCriteriaResult<T>(result, null);
        }

        var queryForCount = query.QueryCriteria.Filter != null 
            ? ApplyFilterCriteria(query.OriginalQuery, query.QueryCriteria.Filter) 
            : query.OriginalQuery;

        var totalCount = await queryExecutor.CountAsync(queryForCount, cancellationToken);

        return new QueryCriteriaResult<T>(result, totalCount);
    }

    public async Task<QueryCriteriaResult<T>> ToResultAsync<T>(
        IQueryable<T> query,
        QueryCriteria queryCriteria,
        CancellationToken cancellationToken = default
    )
    {
        query = queryCriteria.Filter != null 
            ? ApplyFilterCriteria(query, queryCriteria.Filter) 
            : query;

        var requiresTotalCount = queryCriteria.Paging?.TotalCount == true;
        var totalCount = requiresTotalCount 
            ? await queryExecutor.CountAsync(query, cancellationToken) 
            : (int?)null;

        query = queryCriteria.OrderBy != null 
            ? ApplyOrderByCriteria(query, queryCriteria.OrderBy) 
            : query;

        query = queryCriteria.Paging != null 
            ? ApplyPageCriteria(query, queryCriteria.Paging) 
            : query;

        var result = await queryExecutor.ToArrayAsync(query, cancellationToken);

        return new QueryCriteriaResult<T>(result, totalCount);
    }

    private IQueryable<T> ApplyFilterCriteria<T>(IQueryable<T> query, FilterCriteria filter)
    {
        var lambda = expressionBuilder.CreateFilterExpression<T>(filter.Expression);

        query = query.Where(lambda);

        return query;
    }

    private IQueryable<T> ApplyOrderByCriteria<T>(IQueryable<T> query, OrderByCriteria orderBy)
    {        
        if(orderBy.Items.Length == 0)
        {
            return query;
        }

        bool isFirst = true;
        ParameterExpression parameter = expressionBuilder.CreateParameterExpression<T>();
        foreach (var orderByItem in orderBy.Items)
        {   
            var propertyPathExpression = expressionBuilder.CreatePropertyOrFieldExpressionFromPath<T>(orderByItem.PropertyPath, parameter);

            // apply order by item to query
            if (isFirst)
            {
                query = orderByItem.Direction == OrderByCriteria.SortDirection.Ascending
                    ? query.OrderBy(propertyPathExpression)
                    : query.OrderByDescending(propertyPathExpression);
                isFirst = false;
            }
            else
            {
                var orderedQuery = (IOrderedQueryable<T>)query;
                query = orderByItem.Direction == OrderByCriteria.SortDirection.Ascending
                    ? orderedQuery.ThenBy(propertyPathExpression)
                    : orderedQuery.ThenByDescending(propertyPathExpression);
            }
        }

        return query;
    }

    private IQueryable<T> ApplyPageCriteria<T>(IQueryable<T> query, PagingCriteria criteria)
    {
        var defaultPagingCriteria = new PagingCriteria();

        if (criteria.Skip.HasValue)
        {
            query = query.Skip(criteria.Skip.Value);
        }

        if (criteria.Top.HasValue)
        {
            query = query.Take(criteria.Top.Value);
        }

        return query;
    }
}