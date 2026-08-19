using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Mocked;
using Bodde.Query.Test.Models;


namespace Bodde.Query.Test;

public class QueryExecutor_ToResult
{
    private readonly QueryToolkitMock toolkit = new();

    private readonly QueryExecutor sut = new TestQueryExecutor();

    [Theory]
    [InlineData(null)]
    [InlineData(false)]
    public void TotalCount_Not_Required(bool? totalCount)
    {
        var data = EmployeeSetBuilder.Build();
        var queryableWithCriteria = Arrange(data, totalCount, out var expectedData);

        var actual = sut.ToResult(queryableWithCriteria);

        Assert.Equal(expectedData, actual.Items);
        Assert.Null(actual.TotalCount);
    }

    [Fact]
    public void TotalCount_Required()
    {
        var data = EmployeeSetBuilder.Build();
        var queryableWithCriteria = Arrange(data, totalCount: true, out var expectedData);

        var actual = sut.ToResult(queryableWithCriteria);

        Assert.Equal(expectedData, actual.Items);
        Assert.Equal(data.Length, actual.TotalCount);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(false)]
    public async Task Async_TotalCount_Not_Required(bool? totalCount)
    {
        var data = EmployeeSetBuilder.Build();
        var queryableWithCriteria = Arrange(data, totalCount, out var expectedData);

        var actual = await sut.ToResultAsync(queryableWithCriteria);

        Assert.Equal(expectedData, actual.Items);
        Assert.Null(actual.TotalCount);
    }


    [Fact]
    public async Task Async_TotalCount_Required()
    {
        var data = EmployeeSetBuilder.Build();
        var queryableWithCriteria = Arrange(data, totalCount: true, out var expectedData);

        var actual = await sut.ToResultAsync(queryableWithCriteria);

        Assert.Equal(expectedData, actual.Items);
        Assert.Equal(data.Length, actual.TotalCount);
    }


    private QueryWithCriteria<Employee> Arrange(Employee[] data, bool? totalCount, out Employee[] expectedData)
    {
        var dataQuery = EmployeeSetBuilder.Build().AsQueryable();
        var expectedDataQuery = data.Take(3).AsQueryable();
        expectedData = expectedDataQuery.ToArray();
        var criteria = new QueryCriteria(Paging: new(TotalCount: totalCount));

        var queryableWithCriteria = new QueryWithCriteria<Employee>(
            Name: "Test",
            Toolkit: toolkit.Object,
            InputQuery: data.AsQueryable(),
            Criteria: criteria
        );

        // QueryExecutor uses Handler.ApplyCriteria to build the query
        toolkit.Handler
            .Setup(_ => _.ApplyCriteria(queryableWithCriteria.InputQuery, criteria))
            .Returns(expectedDataQuery);

        if (totalCount == true)
        {
            var totalCountCriteria = criteria.ForTotalCount();
            toolkit.Handler
                .Setup(_ => _.ApplyCriteria(queryableWithCriteria.InputQuery, totalCountCriteria))
                .Returns(dataQuery);
        }


        return queryableWithCriteria;
    }

    public class TestQueryExecutor : QueryExecutor
    {
        protected override int Count<T>(IQueryable<T> query)
            => query.Count();

        protected override Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default)
            => Task.FromResult(query.Count());


        protected override T[] ToArray<T>(IQueryable<T> query)
            => query.ToArray();

        protected override Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default)
             => Task.FromResult(query.ToArray());
    }


}
