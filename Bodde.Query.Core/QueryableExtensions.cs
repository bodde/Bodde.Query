using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

public static class QueryableExtensions
{
    extension<T>(IQueryable<T> query)
    {
        public QueryableWithCriteria<T> AsQueryableWithCriteria(IQueryToolkit queryToolkit)
        => new(queryToolkit, query, new QueryCriteria());
    }
}