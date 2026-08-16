using System.Linq.Expressions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

internal class QueryCriteriaHandler(IExpressionBuilder expressionBuilder) : IQueryCriteriaHandler
{
    public IQueryable<T> ApplyCriteria<T>(IQueryable<T> originalQuery, QueryCriteria criteria)
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

        return queryWithCriteria.Select(_ => _);
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