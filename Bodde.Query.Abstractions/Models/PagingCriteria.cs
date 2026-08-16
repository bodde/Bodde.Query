namespace Bodde.Query.Abstractions.Models;

/// <summary>
/// Represents the paging criteria for a query.
/// </summary>
/// <param name="Skip">The number of items to skip.</param>
/// <param name="Top">The maximum number of items to retrieve.</param>
/// <param name="TotalCount">Indicates whether to include the total count.</param>
public record PagingCriteria(int? Skip = null, int? Top = null, bool? TotalCount = null);
