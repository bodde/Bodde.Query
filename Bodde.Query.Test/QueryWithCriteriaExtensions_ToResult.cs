using Bodde.Query.Core;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Mocked;
using Bodde.Query.Test.Models;
using Moq;

namespace Bodde.Query.Test;

public class QueryWithCriteriaExtensions_ToResult
{
    private readonly QueryToolkitMock toolkit;
    private readonly QueryWithCriteria<Employee> sut;

    public QueryWithCriteriaExtensions_ToResult()
    {
        toolkit = new QueryToolkitMock();
        sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);
    }
    
    [Fact]
    public void Test()
    {
        var expected = Arrange();

        var actual = sut.ToResult();

        Assert.Equal(expected, actual);
    }

        [Fact]
    public async Task Async_Test()
    {
        var expected = Arrange();

        var actual = await sut.ToResultAsync();

        Assert.Equal(expected, actual);
    }

    private QueryCriteriaResult<Employee> Arrange()
    {
        var expected = new QueryCriteriaResult<Employee>(
            "Name...",
            "Criteria...",
            sut.OutputQuery.ToArray(),
            sut.InputQuery.Count()
        );

        toolkit.Executor
            .Setup(_ => _.ToResult(sut))
            .Returns(expected);

        
        toolkit.Executor
            .Setup(_ => _.ToResultAsync(sut))
            .ReturnsAsync(expected);

        return expected;
    }
}