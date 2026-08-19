using Bodde.Query.Abstractions.Models;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Mocked;
using Bodde.Query.Test.Models;
using Moq;

namespace Bodde.Query.Test;

public class QueryWithCriteria_ToString
{    
    [Fact]
    public void ToString_ReturnsExpectedString()
    {
        var queryToolkit = new QueryToolkitMock();

        var employees = EmployeeSetBuilder.Build().AsQueryable();
        var queryName = "EmployeeQuery";

        var queryCriteria = new QueryCriteria();

        var expectedFormattedString = "formattedString";
        queryToolkit.Formatter
            .Setup(_ => _.Format(It.Is<QueryCriteria>(_ => _ == queryCriteria)))
            .Returns(expectedFormattedString);

        var sut = new QueryWithCriteria<Employee>(
            queryName,
            queryToolkit.Object,
            employees,
            queryCriteria
        );

        var actual = sut.ToString();

        Assert.Equal($"{queryName} ({expectedFormattedString})", actual);
    }
}