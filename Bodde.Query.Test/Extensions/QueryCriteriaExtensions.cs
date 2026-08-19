using Bodde.Query.Core;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Test.Helpers;

namespace Bodde.Query.Test.Extensions;

public static class QueryCriteriaExtensions
{
    extension(QueryCriteria queryCriteria)
    {
        public QueryCriteria SalaryGreaterThan80000() 
            => queryCriteria.WithFilter(QueryCriteriaItemBuilder.SalaryGreaterThan80000);

        public QueryCriteria OrderByLastName() 
            => queryCriteria.WithOrderBy(new(QueryCriteriaItemBuilder.OrderByLastName));

        public QueryCriteria FirstPage() 
            => queryCriteria.WithPaging(new(Skip: 0, Top: 5, TotalCount: true));
    }
}