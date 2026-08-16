using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

public static class QueryableExtensions
{
    extension<T>(IQueryable<T> query)
    {
        public QueryableWithCriteria<T> WithCriteria(IQueryToolkit queryToolkit)
            => new(string.Empty, queryToolkit, query, new QueryCriteria());

        public QueryableWithCriteria<T> WithCriteria(string name, IQueryToolkit queryToolkit)
            => new(name, queryToolkit, query, new QueryCriteria());

        public QueryableWithCriteria<T> WithCriteria(QueryCriteria criteria, IQueryToolkit queryToolkit)
            => new(string.Empty, queryToolkit, query, criteria);

        public QueryableWithCriteria<T> WithCriteria(string name, QueryCriteria criteria, IQueryToolkit queryToolkit)
            => new(name, queryToolkit, query, criteria);
    }
}