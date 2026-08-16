using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

public interface IQueryCriteriaHandler
{
    IQueryable<T> ApplyCriteria<T>(IQueryable<T> query, QueryCriteria criteria);
}
