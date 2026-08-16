namespace Bodde.Query.Core;

internal class DefaultQueryExecutor : QueryExecutor
{
    protected override int Count<T>(IQueryable<T> query) => query.Count();

    protected override Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default) => Task.FromResult(Count(query));

    protected override T[] ToArray<T>(IQueryable<T> query) => query.ToArray();

    protected override Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default) => Task.FromResult(ToArray(query));
}