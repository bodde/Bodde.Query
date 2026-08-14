using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

public static class QueryableExtensions
{
    extension<T>(IQueryable<T> query)
    {
        public QueryableWithCriteria<T> AsQueryableWithCriteria(IQueryToolkit queryToolkit)
            => new(string.Empty, queryToolkit, query, new QueryCriteria());

        public QueryableWithCriteria<T> AsQueryableWithCriteria(string name, IQueryToolkit queryToolkit)
            => new(name, queryToolkit, query, new QueryCriteria());
    }
}