namespace Bodde.Query.Abstractions.Models;

/// <summary>
/// Represents ordering criteria for a query.
/// </summary>
/// <param name="Items">The ordering items, applied in the specified order.</param>
public sealed record OrderByCriteria(params OrderByCriteria.OrderByItem[] Items)
{
    /// <summary>
    /// Specifies the direction in which values are sorted.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>Sorts values from lowest to highest.</summary>
        Ascending,
        /// <summary>Sorts values from highest to lowest.</summary>
        Descending
    }

    /// <summary>
    /// Represents one property used for ordering.
    /// </summary>
    /// <param name="PropertyPath">The dotted path of the property to sort by.</param>
    /// <param name="Direction">The direction in which to sort.</param>
    public sealed record OrderByItem(string PropertyPath, SortDirection Direction = SortDirection.Ascending);
}
