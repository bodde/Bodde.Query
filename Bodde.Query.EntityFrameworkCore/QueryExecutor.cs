using Bodde.Query.Abstractions.Services;
using Microsoft.EntityFrameworkCore;

namespace Bodde.Query.EntityFrameworkCore;

internal class QueryExecutor : IQueryExecutor
{
    public Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default) => query.CountAsync(ct);

    public Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default) =>  query.ToArrayAsync(ct);
}