using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

public interface IQueryCriteriaHandler
{
    QueryableWithCriteria<T> ApplyCriteria<T>(IQueryable<T> query, QueryCriteria criteria);
 
    Task<QueryCriteriaResult<T>> ToResultAsync<T>(
        QueryableWithCriteria<T> query,
        CancellationToken cancellationToken = default
        );
        
    Task<QueryCriteriaResult<T>> ToResultAsync<T>(
        IQueryable<T> query, 
        QueryCriteria criteria,
        CancellationToken cancellationToken = default
        );
   
}
