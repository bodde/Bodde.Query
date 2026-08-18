using Bodde.Query.Abstractions.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Mocked;
using Bodde.Query.Test.Models;

namespace Bodde.Query.Test;

public class QueryableWithCriteriaExtensions_RequiresTotalCount
{
    private readonly QueryToolkitMock toolkit;
    private readonly QueryableWithCriteria<Employee> sut;

    public QueryableWithCriteriaExtensions_RequiresTotalCount()
    {
        toolkit = new QueryToolkitMock();
        sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);
    }
    
    [Theory]
    [InlineData(null, null, null, false)]
    [InlineData(10, null, null, false)]
    [InlineData(null, 10, null, false)]
    [InlineData(null, null, false, false)]
    [InlineData(null, null, true, true)]
    [InlineData(20, 10, true, true)]
    public void Test(int? skip, int? top, bool? totalCount, bool expectedRequiresTotalCount)
    {
        var actual = sut
            .WithPaging(skip, top, totalCount)
            .RequiresTotalCount();

        Assert.Equal(expectedRequiresTotalCount, actual);
    }

}