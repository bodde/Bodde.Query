
using System.Collections;
using System.Linq.Expressions;

namespace Bodde.Query.Abstractions.Models;

public class QueryableWithCriteria<TItem>(
    IQueryable<TItem> originalQuery, 
    QueryCriteria queryCriteria,
    IQueryable<TItem> queryWithCriteria) 
    : IQueryable<TItem>
{
    public IQueryable<TItem> OriginalQuery { get; } = originalQuery;

    public QueryCriteria QueryCriteria { get; } = queryCriteria;

    public Type ElementType => queryWithCriteria.ElementType;

    public Expression Expression => queryWithCriteria.Expression;

    public IQueryProvider Provider => queryWithCriteria.Provider;

    public IEnumerator<TItem> GetEnumerator() => queryWithCriteria.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => queryWithCriteria.GetEnumerator();
}
