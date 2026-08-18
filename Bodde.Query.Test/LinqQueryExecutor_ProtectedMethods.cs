using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Mocked;
using Bodde.Query.Test.Models;
using Moq;

namespace Bodde.Query.Test;

public class LinqQueryExecutor_ProtectedMethods
{
    private readonly LinqQueryExecutorUnderTest sut = new();

    [Fact]
    public void ToArray()
    {
        var data = EmployeeSetBuilder.Build().AsQueryable();

        var actual = sut.ProtectedToArray(data);

        Assert.Equal(data, actual);
    }

    [Fact]
    public void Count()
    {
        var data = EmployeeSetBuilder.Build().AsQueryable();

        var actual = sut.ProtectedCount(data);

        Assert.Equal(data.Count(), actual);
    }


    [Fact]
    public async Task ToArrayAsync()
    {
        var data = EmployeeSetBuilder.Build().AsQueryable();

        var actual = await sut.ProtectedToArrayAsync(data);

        Assert.Equal(data, actual);
    }

    [Fact]
    public async Task CountAsync()
    {
        var data = EmployeeSetBuilder.Build().AsQueryable();

        var actual = await sut.ProtectedCountAsync(data);

        Assert.Equal(data.Count(), actual);
    }
    

    class LinqQueryExecutorUnderTest() : LinqQueryExecutor
    {
        public int ProtectedCount<T>(IQueryable<T> query) => Count(query);

        public Task<int> ProtectedCountAsync<T>(IQueryable<T> query, CancellationToken ct = default) => CountAsync(query);

        public T[] ProtectedToArray<T>(IQueryable<T> query) => ToArray(query);

        public Task<T[]> ProtectedToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default) => ToArrayAsync(query);
    }
}
