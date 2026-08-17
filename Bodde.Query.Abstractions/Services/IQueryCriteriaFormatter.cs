using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

public interface IQueryCriteriaFormatter
{
    string FormatPaging(PagingCriteria paging);

    string FormatFilter(FilterCriteria filter);

    string FormatOrderBy(OrderByCriteria orderBy);

    string Format(QueryCriteria criteria);
}
