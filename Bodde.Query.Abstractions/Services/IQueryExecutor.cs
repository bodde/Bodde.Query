using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

public interface IQueryExecutor
{   
    QueryCriteriaResult<T> ToResult<T>(QueryableWithCriteria<T> query);

    Task<QueryCriteriaResult<T>> ToResultAsync<T>(
        QueryableWithCriteria<T> query,
        CancellationToken cancellationToken = default
        );
}
