using Bodde.Query.Core;
using Bodde.Query.Test.Helpers;

namespace Bodde.Query.Test;

public class DefaultQueryExecutor_CountAsync
{
    [Fact]
    public async Task CountAsync_ReturnsExpectedCount()
    {
        var employees = EmployeeSetBuilder.Build().AsQueryable();
        var sut = new DefaultQueryExecutor();

        var actual = await sut.CountAsync(employees);

        Assert.Equal(employees.Count(), actual);
    }
}