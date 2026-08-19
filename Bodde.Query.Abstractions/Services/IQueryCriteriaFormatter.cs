using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

/// <summary>
/// The service that formats query criteria as strings.
/// </summary>
public interface IQueryCriteriaFormatter
{
    /// <summary>
    /// Formats paging criteria as a query string fragment.
    /// </summary>
    /// <param name="paging">The paging criteria to format.</param>
    /// <returns>The formatted paging criteria.</returns>
    string FormatPaging(PagingCriteria paging);

    /// <summary>
    /// Formats filter criteria as a query string fragment.
    /// </summary>
    /// <param name="filter">The filter criteria to format.</param>
    /// <returns>The formatted filter criteria.</returns>
    string FormatFilter(FilterCriteria filter);

    /// <summary>
    /// Formats ordering criteria as a query string fragment.
    /// </summary>
    /// <param name="orderBy">The ordering criteria to format.</param>
    /// <returns>The formatted ordering criteria.</returns>
    string FormatOrderBy(OrderByCriteria orderBy);

    /// <summary>
    /// Formats all query criteria as a query string.
    /// </summary>
    /// <param name="criteria">The query criteria to format.</param>
    /// <returns>The formatted query criteria.</returns>
    string Format(QueryCriteria criteria);
}
