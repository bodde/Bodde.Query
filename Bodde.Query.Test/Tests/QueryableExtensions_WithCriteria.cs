using Bodde.Query.Core;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Models;
using Moq;

namespace Bodde.Query.Test;

public class QueryableExtensions_WithCriteria
{
    private readonly Mock<IQueryToolkit> queryToolkit;

    public QueryableExtensions_WithCriteria()
    {
        queryToolkit = new Mock<IQueryToolkit>();
    }

    [Fact]
    public async Task Default()
    {
        var employees = EmployeeSetBuilder.Build().AsQueryable();

        var emptyCriteria = new QueryCriteria();

        var sut = employees.WithCriteria(queryToolkit.Object);

        Assert.NotNull(sut);
        Assert.IsType<QueryWithCriteria<Employee>>(sut);
        Assert.Equal(queryToolkit.Object, sut.Toolkit);
        Assert.Empty(sut.Name);
        Assert.Equal(emptyCriteria, sut.Criteria);
        Assert.Equal(employees, sut.InputQuery);
    }

    [Fact]
    public async Task Custom_Name()
    {
        var employees = EmployeeSetBuilder.Build().AsQueryable();

        var emptyCriteria = new QueryCriteria();
        var customName = "CustomQueryName";

        var sut = employees.WithCriteria(customName,queryToolkit.Object);

        Assert.NotNull(sut);
        Assert.IsType<QueryWithCriteria<Employee>>(sut);
        Assert.Equal(queryToolkit.Object, sut.Toolkit);
        Assert.Equal(customName, sut.Name);
        Assert.Equal(emptyCriteria, sut.Criteria);
        Assert.Equal(employees, sut.InputQuery);
    }

    [Fact]
    public async Task Custom_Criteria()
    {
        var employees = EmployeeSetBuilder.Build().AsQueryable();
        var salaryGreaterThan8000 = new FilterCriteria.ComparisonExpression("Salary", FilterCriteria.ComparisonOperator.GreaterThan, 80000);

        var customCriteria = new QueryCriteria(new(salaryGreaterThan8000));

        var sut = employees.WithCriteria(customCriteria, queryToolkit.Object);

        Assert.NotNull(sut);
        Assert.IsType<QueryWithCriteria<Employee>>(sut);
        Assert.Equal(queryToolkit.Object, sut.Toolkit);
        Assert.Empty(sut.Name);
        Assert.Equal(customCriteria, sut.Criteria);
        Assert.Equal(employees, sut.InputQuery);
    }

    [Fact]
    public async Task Custom_Name_And_Criteria()
    {
        var employees = EmployeeSetBuilder.Build().AsQueryable();
        var salaryGreaterThan8000 = new FilterCriteria.ComparisonExpression("Salary", FilterCriteria.ComparisonOperator.GreaterThan, 80000);

        var customCriteria = new QueryCriteria(new(salaryGreaterThan8000));
        var customName = "CustomQueryName";

        var sut = employees.WithCriteria(customName, customCriteria, queryToolkit.Object);

        Assert.NotNull(sut);
        Assert.IsType<QueryWithCriteria<Employee>>(sut);
        Assert.Equal(queryToolkit.Object, sut.Toolkit);
        Assert.Equal(customName, sut.Name);
        Assert.Equal(customCriteria, sut.Criteria);
        Assert.Equal(employees, sut.InputQuery);
    }
}