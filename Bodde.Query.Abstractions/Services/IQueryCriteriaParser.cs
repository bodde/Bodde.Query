using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

public interface IQueryCriteriaParser
{
    QueryCriteria Parse(string criteriaString);

    QueryCriteria Parse(string? filter = null, string? orderBy = null, int? skip = null, int? top = null, bool? totalCount = null);
    
    QueryCriteria Parse(QueryCriteriaParameters queryCriteriaParameters);

    PagingCriteria ParsePaging(string pagingString);

    FilterCriteria ParseFilter(string filterString);

    FilterCriteria.FilterExpression ParseFilterExpression(string filterString);

    OrderByCriteria ParseOrderBy(string orderByString);

    OrderByCriteria.OrderByItem[] ParseOrderByItems(string orderByString);
}
