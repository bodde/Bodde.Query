namespace Bodde.Query.Abstractions.Models;

public record QueryCriteriaResult<T>(QueryCriteria Criteria, T[] Items, int? TotalCount = null);
