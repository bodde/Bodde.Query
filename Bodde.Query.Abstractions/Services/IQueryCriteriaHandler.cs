using System;
using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

public interface IQueryCriteriaHandler
{
    QueryCriteriaQueryable<T> ApplyCriteria<T>(IQueryable<T> query, QueryCriteria criteria);
 
    Task<QueryCriteriaResult<T>> ToResultAsync<T>(
        QueryCriteriaQueryable<T> query,
        CancellationToken cancellationToken = default
        );
        
    Task<QueryCriteriaResult<T>> ToResultAsync<T>(
        IQueryable<T> query, 
        QueryCriteria criteria,
        CancellationToken cancellationToken = default
        );
   
}
