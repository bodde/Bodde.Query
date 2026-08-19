namespace Bodde.Query.Abstractions.Models;

/// <summary>
/// Represents the filter, ordering, and paging criteria for a query.
/// </summary>
/// <param name="Filter">The filter criteria, if any.</param>
/// <param name="OrderBy">The ordering criteria, if any.</param>
/// <param name="Paging">The paging criteria, if any.</param>
public record QueryCriteria(
    FilterCriteria? Filter = null, 
    OrderByCriteria? OrderBy = null, 
    PagingCriteria? Paging = null
    );
