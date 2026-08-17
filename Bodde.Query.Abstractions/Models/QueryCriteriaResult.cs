namespace Bodde.Query.Abstractions.Models;

public record QueryCriteriaResult<T>(string Name, string Criteria, T[] Items, int? TotalCount = null);
