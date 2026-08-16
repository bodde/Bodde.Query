using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class QueryCriteriaQueryable_IQueryable
{
    [Fact]
    public void IQueryable_Implemented()
    {
        var queryCriteria = new QueryCriteria();
        var queryToolkit = QueryToolkit.Default();
        var originalQuery = Array.Empty<int>().AsQueryable();
        var sut = new QueryableWithCriteria<int>("Test", queryToolkit, originalQuery, queryCriteria);

        var actualQueryCriteria = sut.Criteria;
        var actualElementType = sut.ElementType;
        var actualExpression = sut.Expression;
        var actualProvider = sut.Provider;
        var actualEnumerator = sut.GetEnumerator();
        var actualEnumeratorNonGeneric = ((System.Collections.IEnumerable)sut).GetEnumerator();

        Assert.Equal(queryCriteria, actualQueryCriteria);
        Assert.Equal(originalQuery.ElementType, actualElementType);
        Assert.Equal(originalQuery.Expression, actualExpression);
        Assert.Equal(originalQuery.Provider, actualProvider);
        Assert.NotNull(actualEnumerator);
        Assert.NotNull(actualEnumeratorNonGeneric);
    }
}
