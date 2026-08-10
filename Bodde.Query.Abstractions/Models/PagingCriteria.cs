namespace Bodde.Query.Abstractions.Models;

public record PagingCriteria(int? Skip = 0, int? Top = 10, bool? TotalCount = false);
