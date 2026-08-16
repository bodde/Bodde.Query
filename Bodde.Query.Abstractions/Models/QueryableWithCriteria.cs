
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
    private Lazy<IQueryable<TItem>> queryableWithCriteria = new Lazy<IQueryable<TItem>>(() => Toolkit.Handler.ApplyCriteria(Queryable, Criteria));

    public Type ElementType => queryableWithCriteria.Value.ElementType;

    public Expression Expression => queryableWithCriteria.Value.Expression;

    public IQueryProvider Provider => queryableWithCriteria.Value.Provider;

    public IEnumerator<TItem> GetEnumerator() => queryableWithCriteria.Value.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => queryableWithCriteria.Value.GetEnumerator();

    public override string ToString()
    {
        var name = string.IsNullOrWhiteSpace(Name) ? "<unnamed>" : Name;
        var criteria = Toolkit.Formatter.Format(Criteria);

        return $"{name} ({criteria})";
    }

    // public void Deconstruct(out string name, out IQueryable<TItem> queryable, out QueryCriteria criteria, out IQueryToolkit toolkit)
    // {
    //     name = Name;
    //     queryable = Queryable;
    //     criteria = Criteria;
    //     toolkit = Toolkit;
    // }

}
