
using System.Collections;
using System.Linq.Expressions;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Abstractions.Models;

public record QueryableWithCriteria<TItem>(
    string Name,
    IQueryToolkit Toolkit,
    IQueryable<TItem> Queryable, 
    QueryCriteria Criteria
    ) 
    : IQueryable<TItem>
{
    IQueryable<TItem> queryableWithCriteria = Toolkit.Handler.ApplyCriteria(Queryable, Criteria);

    public Type ElementType => queryableWithCriteria.ElementType;

    public Expression Expression => queryableWithCriteria.Expression;

    public IQueryProvider Provider => queryableWithCriteria.Provider;

    public IEnumerator<TItem> GetEnumerator() => queryableWithCriteria.GetEnumerator();

    public override string ToString()
    {
        var name = string.IsNullOrWhiteSpace(Name) ? "<unnamed>" : Name;
        var criteria = Toolkit.Formatter.Format(Criteria);

        return $"{name} ({criteria})";
    }

    IEnumerator IEnumerable.GetEnumerator() => queryableWithCriteria.GetEnumerator();
}
