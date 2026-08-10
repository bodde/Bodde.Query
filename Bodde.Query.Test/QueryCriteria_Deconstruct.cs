using Bodde.Query.Abstractions.Models;
using COp = Bodde.Query.Abstractions.Models.FilterCriteria.ComparisonOperator;

namespace Bodde.Query.Test;

public class QueryCriteria_Deconstruct
{
    [Fact]
    public void Deconstruct_Yields_Filter_OrderBy_And_Paging()
    {
        var filterExpression = new FilterCriteria.ComparisonExpression("Age", COp.GreaterThan, 30);
        var filter = new FilterCriteria(filterExpression);
        var orderBy = new OrderByCriteria();
        var paging = new PagingCriteria();
        var c = new QueryCriteria(filter, orderBy, paging);
        var (f, o, p) = c;
        Assert.Equal(filter, f);
        Assert.Equal(orderBy, o);
        Assert.Equal(paging, p);
    }

    [Fact]
    public void Deconstruct_Yields_Nulls_When_Properties_Are_Null()
    {
        var c = new QueryCriteria();
        var (f, o, p) = c;
        Assert.Null(f);
        Assert.Null(o);
        Assert.Null(p);
    }
}
