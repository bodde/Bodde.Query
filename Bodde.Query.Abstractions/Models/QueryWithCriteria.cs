using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Abstractions.Models;

/// <summary>
/// Wraps an input query together with its toolkit and query criteria.
/// </summary>
/// <typeparam name="TItem">The type of the query elements.</typeparam>
/// <param name="Name">The name of the query.</param>
/// <param name="Toolkit">The toolkit used to process the query.</param>
/// <param name="InputQuery">The source query.</param>
/// <param name="Criteria">The criteria associated with the query.</param>
public record QueryWithCriteria<TItem>(
    string Name,
    IQueryToolkit Toolkit,
    IQueryable<TItem> InputQuery, 
    QueryCriteria Criteria
    ) 
{
    private readonly Lazy<IQueryable<TItem>> outputQuery = new(() => Toolkit.Handler.ApplyCriteria(InputQuery, Criteria));

    private readonly Lazy<string> formattedCriteria = new(() => Toolkit.Formatter.Format(Criteria));

    /// <summary>
    /// Gets the input query with the criteria applied.
    /// </summary>
    public IQueryable<TItem> OutputQuery => outputQuery.Value;

    /// <summary>
    /// Gets the criteria formatted as a query string.
    /// </summary>
    public string FormattedCriteria => formattedCriteria.Value;

    /// <summary>
    /// Returns a string representation containing the query name and formatted criteria.
    /// </summary>
    /// <returns>The query name and formatted criteria.</returns>
    public override string ToString()
    {
        var name = string.IsNullOrWhiteSpace(Name) ? "<unnamed>" : Name;

        return $"{name} ({FormattedCriteria})";
    }
}
