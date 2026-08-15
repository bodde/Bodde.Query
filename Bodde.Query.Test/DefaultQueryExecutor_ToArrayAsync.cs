using Bodde.Query.Core;
using Bodde.Query.Test.Helpers;

namespace Bodde.Query.Test;

public class DefaultQueryExecutor_ToArrayAsync
{
    [Fact]
    public async Task ToArrayAsync_ReturnsExpectedArray()
    {
        var employees = EmployeeSetBuilder.Build().AsQueryable();
        var sut = new DefaultQueryExecutor();

        var actual = await sut.ToArrayAsync(employees);

        Assert.Equal(employees, actual);
    }
}