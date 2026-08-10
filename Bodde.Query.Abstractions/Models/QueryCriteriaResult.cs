namespace Bodde.Query.Abstractions.Models;

public record QueryCriteriaResult<T>(T[] Items, int? TotalCount = null)
{
	public void Deconstruct(out T[] items, out int? totalCount)
	{
		items = Items;
		totalCount = TotalCount;
	}
}
