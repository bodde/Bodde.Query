using Bodde.Query.Abstractions.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Models;

namespace Bodde.Query.Test;

public class QueryableExtensions_WithCriteria
{
    [Fact]
    public async Task Default()
    {
        var employees = EmployeeSetBuilder.Build().AsQueryable();

        var queryToolkit = QueryToolkit.Default();
        var emptyCriteria = new QueryCriteria();

        var sut = employees.WithCriteria(queryToolkit);

        Assert.NotNull(sut);
        Assert.IsType<QueryableWithCriteria<Employee>>(sut);
        Assert.Equal(queryToolkit, sut.Toolkit);
        Assert.Empty(sut.Name);
        Assert.Equal(emptyCriteria, sut.Criteria);
        Assert.Equal(employees, sut.InputQuery);
    }

    [Fact]
    public async Task Custom_Name()
    {
        var employees = EmployeeSetBuilder.Build().AsQueryable();

        var queryToolkit = QueryToolkit.Default();
        var emptyCriteria = new QueryCriteria();
        var customName = "CustomQueryName";

        var sut = employees.WithCriteria(customName,queryToolkit);

        Assert.NotNull(sut);
        Assert.IsType<QueryableWithCriteria<Employee>>(sut);
        Assert.Equal(queryToolkit, sut.Toolkit);
        Assert.Equal(customName, sut.Name);
        Assert.Equal(emptyCriteria, sut.Criteria);
        Assert.Equal(employees, sut.InputQuery);
    }

    [Fact]
    public async Task Custom_Criteria()
    {
        var employees = EmployeeSetBuilder.Build().AsQueryable();
        var salaryGreaterThan8000 = new FilterCriteria.ComparisonExpression("Salary", FilterCriteria.ComparisonOperator.GreaterThan, 80000);

        var queryToolkit = QueryToolkit.Default();
        var customCriteria = new QueryCriteria(new(salaryGreaterThan8000));

        var sut = employees.WithCriteria(customCriteria, queryToolkit);

        Assert.NotNull(sut);
        Assert.IsType<QueryableWithCriteria<Employee>>(sut);
        Assert.Equal(queryToolkit, sut.Toolkit);
        Assert.Empty(sut.Name);
        Assert.Equal(customCriteria, sut.Criteria);
        Assert.Equal(employees, sut.InputQuery);
    }

    [Fact]
    public async Task Custom_Name_And_Criteria()
    {
        var employees = EmployeeSetBuilder.Build().AsQueryable();
        var salaryGreaterThan8000 = new FilterCriteria.ComparisonExpression("Salary", FilterCriteria.ComparisonOperator.GreaterThan, 80000);

        var queryToolkit = QueryToolkit.Default();
        var customCriteria = new QueryCriteria(new(salaryGreaterThan8000));
        var customName = "CustomQueryName";

        var sut = employees.WithCriteria(customName, customCriteria, queryToolkit);

        Assert.NotNull(sut);
        Assert.IsType<QueryableWithCriteria<Employee>>(sut);
        Assert.Equal(queryToolkit, sut.Toolkit);
        Assert.Equal(customName, sut.Name);
        Assert.Equal(customCriteria, sut.Criteria);
        Assert.Equal(employees, sut.InputQuery);
    }
}