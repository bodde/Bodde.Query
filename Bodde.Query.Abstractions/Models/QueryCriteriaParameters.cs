namespace Bodde.Query.Abstractions.Models;

public record QueryCriteriaParameters(
    int? Skip = null,
    int? Top = null,
    bool? Count = null,
    string? Filter = null,
    string? OrderBy = null
);
