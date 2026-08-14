using System;
using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Test;

public class QueryCriteriaQueryable_IQueryable
{
    [Fact]
    public void IQueryable_Implemented()
    {
        var queryCriteria = new QueryCriteria();
        var queryToolkit = new DefaultQueryToolkit();
        var originalQuery = Array.Empty<int>().AsQueryable();
        var queryWithCriteria = Array.Empty<int>().AsQueryable();
        var sut = new QueryableWithCriteria<int>(queryToolkit, originalQuery, queryCriteria, queryWithCriteria);

        var actualQueryCriteria = sut.QueryCriteria;
        var actualElementType = sut.ElementType;
        var actualExpression = sut.Expression;
        var actualProvider = sut.Provider;
        var actualEnumerator = sut.GetEnumerator();
        var actualEnumeratorNonGeneric = ((System.Collections.IEnumerable)sut).GetEnumerator();

        Assert.Equal(queryCriteria, actualQueryCriteria);
        Assert.Equal(queryWithCriteria.ElementType, actualElementType);
        Assert.Equal(queryWithCriteria.Expression, actualExpression);
        Assert.Equal(queryWithCriteria.Provider, actualProvider);
        Assert.NotNull(actualEnumerator);
        Assert.NotNull(actualEnumeratorNonGeneric);
    }
}
