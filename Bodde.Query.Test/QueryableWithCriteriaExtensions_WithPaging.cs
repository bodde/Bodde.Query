using Bodde.Query.Abstractions.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Mocked;
using Bodde.Query.Test.Models;

namespace Bodde.Query.Test;

public class QueryableWithCriteriaExtensions_WithPaging
{
    private readonly QueryToolkitMock toolkit;
    private readonly QueryableWithCriteria<Employee> sut;

    public QueryableWithCriteriaExtensions_WithPaging()
    {
        toolkit = new QueryToolkitMock();
        sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);
    }
    
    [Theory]
    [InlineData(null, null, null)]
    [InlineData(10, null, null)]
    [InlineData(null, 10, null)]
    [InlineData(null, null, false)]
    [InlineData(null, null, true)]
    [InlineData(20, 10, true)]
    public void Test(int? skip, int? top, bool? totalCount)
    {
        var actual = sut.WithPaging(skip, top, totalCount);

        Assert.NotNull(actual);
        Assert.NotNull(actual.Criteria);
        Assert.NotNull(actual.Criteria.Paging);
        Assert.Equal(skip, actual.Criteria.Paging.Skip);
        Assert.Equal(top, actual.Criteria.Paging.Top);
        Assert.Equal(totalCount, actual.Criteria.Paging.TotalCount);
    }

}