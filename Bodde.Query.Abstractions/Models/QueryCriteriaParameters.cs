namespace Bodde.Query.Abstractions.Models;

public record QueryCriteriaParameters(
    int? Skip,
    int? Top,
    bool? Count,
    string? Filter,
    string? OrderBy
)
{
    public bool AreSet =>
        Skip.HasValue ||
        Top.HasValue ||
        Count.HasValue ||
        !string.IsNullOrWhiteSpace(Filter) ||
        !string.IsNullOrWhiteSpace(OrderBy);
}
