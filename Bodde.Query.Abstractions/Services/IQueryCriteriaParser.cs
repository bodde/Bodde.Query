using System;
using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

public interface IQueryCriteriaParser
{
    PagingCriteria? ParsePaging(string pagingString);

    FilterCriteria? ParseFilter(string filterString);

    FilterCriteria.FilterExpression? ParseFilterExpression(string filterString);

    OrderByCriteria? ParseOrderBy(string orderByString);

    OrderByCriteria.OrderByItem[] ParseOrderByItems(string orderByString);

    QueryCriteria? Parse(string criteriaString);
    
    QueryCriteria? Parse(QueryCriteriaParameters? queryCriteriaParameters);
}
