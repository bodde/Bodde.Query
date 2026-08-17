using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Abstractions.Models;

public record QueryableWithCriteria<TItem>(
    string Name,
    IQueryToolkit Toolkit,
    IQueryable<TItem> InputQuery, 
    QueryCriteria Criteria
    ) 
{
    private readonly Lazy<IQueryable<TItem>> outputQueryable = new(() => Toolkit.Handler.ApplyCriteria(InputQuery, Criteria));

    public IQueryable<TItem> OutputQuery => outputQueryable.Value;

    public string FormattedCriteria => Toolkit.Formatter.Format(Criteria);

    public override string ToString()
    {
        var name = string.IsNullOrWhiteSpace(Name) ? "<unnamed>" : Name;

        return $"{name} ({FormattedCriteria})";
    }
}
