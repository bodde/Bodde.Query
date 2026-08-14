
using System.Collections;
using System.Linq.Expressions;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Abstractions.Models;

public record QueryableWithCriteria<TItem>(
    IQueryToolkit Toolkit,
    IQueryable<TItem> Queryable, 
    QueryCriteria Criteria) 
    : IQueryable<TItem>
{
    IQueryable<TItem> queryableWithCriteria = Toolkit.Handler.ApplyCriteria(Queryable, Criteria);

    public Type ElementType => queryableWithCriteria.ElementType;

    public Expression Expression => queryableWithCriteria.Expression;

    public IQueryProvider Provider => queryableWithCriteria.Provider;

    public IEnumerator<TItem> GetEnumerator() => queryableWithCriteria.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => queryableWithCriteria.GetEnumerator();
}
