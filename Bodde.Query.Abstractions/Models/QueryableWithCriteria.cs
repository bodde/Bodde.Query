
using System.Collections;
using System.Linq.Expressions;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Abstractions.Models;

public class QueryableWithCriteria<TItem>(
    IQueryToolkit toolkit,
    IQueryable<TItem> queryable, 
    QueryCriteria queryCriteria,
    IQueryable<TItem> queryableWithCriteria) 
    : IQueryable<TItem>
{
    public IQueryToolkit Toolkit { get; } = toolkit;

    public IQueryable<TItem> OriginalQueryable { get; } = queryable;

    public QueryCriteria QueryCriteria { get; } = queryCriteria;

    public Type ElementType => queryableWithCriteria.ElementType;

    public Expression Expression => queryableWithCriteria.Expression;

    public IQueryProvider Provider => queryableWithCriteria.Provider;

    public IEnumerator<TItem> GetEnumerator() => queryableWithCriteria.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => queryableWithCriteria.GetEnumerator();
}
