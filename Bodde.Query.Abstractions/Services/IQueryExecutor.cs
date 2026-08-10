namespace Bodde.Query.Abstractions.Services;

public interface IQueryExecutor
{
    Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default);

    Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default);
}
