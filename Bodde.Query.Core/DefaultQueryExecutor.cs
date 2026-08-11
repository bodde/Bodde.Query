using Bodde.Query.Abstractions.Services;

internal class DefaultQueryExecutor : IQueryExecutor
    {
        public Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default)
        {
            return Task.FromResult(query.Count());
        }

        public Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default)
        {
            return Task.FromResult(query.ToArray());
        }
    }