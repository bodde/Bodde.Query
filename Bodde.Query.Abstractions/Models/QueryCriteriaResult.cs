namespace Bodde.Query.Abstractions.Models;

public record QueryCriteriaResult<T>(string Name, QueryCriteria Criteria, T[] Items, int? TotalCount = null);
