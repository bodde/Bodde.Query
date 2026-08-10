namespace Bodde.Query.Abstractions.Models;

public sealed record OrderByCriteria(params OrderByCriteria.OrderByItem[] Items)
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public sealed record OrderByItem(string PropertyPath, SortDirection Direction = SortDirection.Ascending);
}
