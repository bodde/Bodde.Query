using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

/// <summary>
/// The service that parses query criteria from strings or query parameters.
/// </summary>
public interface IQueryCriteriaParser
{
    /// <summary>
    /// Parses a complete query criteria string.
    /// </summary>
    /// <param name="criteriaString">The string containing the query criteria.</param>
    /// <returns>The parsed query criteria.</returns>
    QueryCriteria Parse(string criteriaString);

    /// <summary>
    /// Parses query criteria supplied as individual values.
    /// </summary>
    /// <param name="filter">The filter expression, if any.</param>
    /// <param name="orderBy">The ordering expression, if any.</param>
    /// <param name="skip">The number of items to skip, if specified.</param>
    /// <param name="top">The maximum number of items to return, if specified.</param>
    /// <param name="totalCount">Whether to include the total count, if specified.</param>
    /// <returns>The parsed query criteria.</returns>
    QueryCriteria Parse(string? filter = null, string? orderBy = null, int? skip = null, int? top = null, bool? totalCount = null);
    
    /// <summary>
    /// Parses query criteria parameters.
    /// </summary>
    /// <param name="queryCriteriaParameters">The parameters to parse.</param>
    /// <returns>The parsed query criteria.</returns>
    QueryCriteria Parse(QueryCriteriaParams queryCriteriaParameters);

    /// <summary>
    /// Parses a paging criteria string.
    /// </summary>
    /// <param name="pagingString">The string containing the paging criteria.</param>
    /// <returns>The parsed paging criteria.</returns>
    PagingCriteria ParsePaging(string pagingString);

    /// <summary>
    /// Parses a filter criteria string.
    /// </summary>
    /// <param name="filterString">The string containing the filter criteria.</param>
    /// <returns>The parsed filter criteria.</returns>
    FilterCriteria ParseFilter(string filterString);

    /// <summary>
    /// Parses a filter expression string.
    /// </summary>
    /// <param name="filterString">The string containing the filter expression.</param>
    /// <returns>The parsed filter expression.</returns>
    FilterCriteria.FilterExpression ParseFilterExpression(string filterString);

    /// <summary>
    /// Parses an ordering criteria string.
    /// </summary>
    /// <param name="orderByString">The string containing the ordering criteria.</param>
    /// <returns>The parsed ordering criteria.</returns>
    OrderByCriteria ParseOrderBy(string orderByString);
}
