namespace Bodde.Query.Abstractions.Models;

/// <summary>
/// Represents query criteria supplied as individual parameters.
/// </summary>
/// <param name="Skip">The number of items to skip.</param>
/// <param name="Top">The maximum number of items to return.</param>
/// <param name="Count">Whether to include the total count.</param>
/// <param name="Filter">The filter expression, if any.</param>
/// <param name="OrderBy">The ordering expression, if any.</param>
public record QueryCriteriaParams(
    int? Skip = null,
    int? Top = null,
    bool? Count = null,
    string? Filter = null,
    string? OrderBy = null
);
