using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

public abstract class QueryExecutor : IQueryExecutor
{
    protected abstract T[] ToArray<T>(IQueryable<T> query);

    protected abstract int Count<T>(IQueryable<T> query);

    protected abstract Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default);

    protected abstract Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default);

    public QueryCriteriaResult<T> ToResult<T>(QueryableWithCriteria<T> query)
    {       
        var result = ToArray(query);
        int? totalCount = query.RequiresTotalCount() ? Count(query.ForTotalCount()) : null;

        return new QueryCriteriaResult<T>(query.Name, query.Criteria, result, totalCount);
    }

    public async Task<QueryCriteriaResult<T>> ToResultAsync<T>(QueryableWithCriteria<T> query, CancellationToken cancellationToken = default)
    {        
        var result = await ToArrayAsync(query, cancellationToken);
        int? totalCount = query.RequiresTotalCount() ? await CountAsync(query.ForTotalCount(), cancellationToken) : null;

        return new QueryCriteriaResult<T>(query.Name, query.Criteria, result, totalCount);
    }
    
}