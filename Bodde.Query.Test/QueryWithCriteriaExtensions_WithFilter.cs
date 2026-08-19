using Bodde.Query.Abstractions.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Mocked;
using Bodde.Query.Test.Models;
using Moq;
using static Bodde.Query.Abstractions.Models.FilterCriteria;

namespace Bodde.Query.Test;

public class QueryWithCriteriaExtensions_WithFilter
{
    private readonly QueryToolkitMock toolkit;
    private readonly QueryWithCriteria<Employee> sut;

    public QueryWithCriteriaExtensions_WithFilter()
    {
        toolkit = new QueryToolkitMock();
        sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    [Fact]
    public void NullStatement_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => sut.WithFilter(null));
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    
    [Fact]
    public void Add_FilterCriteria()
    {
        var filterStatement = "Salary gt 80000";
        var filterExpression = QueryCriteriaItemBuilder.SalaryGreaterThan80000;

        toolkit.Parser
            .Setup(_ => _.ParseFilterExpression(It.Is<string>(p => p == filterStatement)))
            .Returns(filterExpression);

        var actual = sut.WithFilter(filterStatement);

        var expectedFilterCriteria = new FilterCriteria(filterExpression);

        Assert.NotNull(actual);
        Assert.NotNull(actual.Criteria);
        Assert.NotNull(actual.Criteria.Filter);
        Assert.Equal(expectedFilterCriteria, actual.Criteria.Filter);

        toolkit.Parser.Verify(_ => _.ParseFilterExpression(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void Add_FilterCriteria_Twice()
    {
        var firstFilterStatement = "Salary gt 80000";
        var firstFilterExpression = QueryCriteriaItemBuilder.SalaryGreaterThan80000;

        var secondFilterStatement = "Role eq 'Manager'";
        var secondFilterExpression = QueryCriteriaItemBuilder.RoleManager;

        toolkit.Parser
            .Setup(_ => _.ParseFilterExpression(It.Is<string>(p => p == firstFilterStatement)))
            .Returns(firstFilterExpression);

        toolkit.Parser
            .Setup(_ => _.ParseFilterExpression(It.Is<string>(p => p == secondFilterStatement)))
            .Returns(secondFilterExpression);

        var actual = sut
            .WithFilter(firstFilterStatement)
            .WithFilter(secondFilterStatement);

        var expectedFilterExpression = new LogicalExpression(LogicalOperator.And, firstFilterExpression, secondFilterExpression);
        var expectedFilterCriteria = new FilterCriteria(expectedFilterExpression);

        Assert.NotNull(actual);
        Assert.NotNull(actual.Criteria);
        Assert.NotNull(actual.Criteria.Filter);
        Assert.Equal(expectedFilterCriteria, actual.Criteria.Filter);

        toolkit.Parser.Verify(_ => _.ParseFilterExpression(It.IsAny<string>()), Times.Exactly(2));
    }
}