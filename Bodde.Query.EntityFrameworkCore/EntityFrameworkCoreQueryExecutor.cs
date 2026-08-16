using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Microsoft.EntityFrameworkCore;

namespace Bodde.Query.EntityFrameworkCore;

internal class EntityFrameworkCoreQueryExecutor : QueryExecutor
{
    protected override int Count<T>(IQueryable<T> query) => query.Count();

    protected override Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default) => query.CountAsync(ct);

    protected override T[] ToArray<T>(IQueryable<T> query) => query.ToArray();


    protected override Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default) => query.ToArrayAsync(ct);
}