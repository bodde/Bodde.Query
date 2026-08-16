using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Models;

namespace Bodde.Query.Test;

public class QueryableWithCriteria_ToString
{
    [Fact]
    public void ToString_ReturnsExpectedString()
    {
        var toolkit = QueryToolkit.Default();

        var employees = EmployeeSetBuilder.Build().AsQueryable();
        var queryName = "EmployeeQuery";

        var salaryGreaterThan8000 = new FilterCriteria.ComparisonExpression("Salary", FilterCriteria.ComparisonOperator.GreaterThan, 80000);
        var orderByLastName = new OrderByCriteria.OrderByItem("LastName", OrderByCriteria.SortDirection.Ascending);
        var queryCriteria = new QueryCriteria
        {
            Filter = new (salaryGreaterThan8000),
            OrderBy = new(orderByLastName),
            Paging = new(Skip: 0, Top: 5, TotalCount: true)
        };

        var expectedFormattedCriteria = toolkit.Formatter.Format(queryCriteria);

        var sut = new QueryableWithCriteria<Employee>(
            queryName,
            toolkit,
            employees,
            queryCriteria
        );

        var actual = sut.ToString();

        Assert.Equal($"{queryName} ({expectedFormattedCriteria})", actual);
    }
}