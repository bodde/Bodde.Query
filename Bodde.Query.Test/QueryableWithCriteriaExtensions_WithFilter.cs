using Bodde.Query.Abstractions.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;
using Bodde.Query.Test.Helpers;
using Moq;
using static Bodde.Query.Abstractions.Models.FilterCriteria;

namespace Bodde.Query.Test;

public class QueryableWithCriteriaExtensions_WithFilter
{    
    [Fact]
    public void NullStatement_Throw()
    {
        var toolkit = new Mock<IQueryToolkit>();

        var sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => sut.WithFilter(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }
    
    [Fact]
    public void Add_FilterCriteria()
    {
        var filterExpression = new ComparisonExpression("Salary", ComparisonOperator.GreaterThan, 80000);
        var filterStatement = "Salary gt 80000";

        var parser = new Mock<IQueryCriteriaParser>();
        parser.Setup(_ => _.ParseFilterExpression(It.Is<string>(p => p == filterStatement)))
            .Returns(filterExpression);

        var toolkit = new Mock<IQueryToolkit>();
        toolkit.SetupGet(_ => _.Parser).Returns(parser.Object);

        var sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);

        var actual = sut.WithFilter(filterStatement);

        var expectedFilterCriteria = new FilterCriteria(filterExpression);

        Assert.NotNull(actual);
        Assert.NotNull(actual.Criteria);
        Assert.NotNull(actual.Criteria.Filter);
        Assert.Equal(expectedFilterCriteria, actual.Criteria.Filter);

        parser.Verify(_ => _.ParseFilterExpression(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void Add_FilterCriteria_Twice()
    {
        var firstFilterExpression = new ComparisonExpression("Salary", ComparisonOperator.GreaterThan, 80000);
        var firstFilterStatement = "Salary gt 80000";

        var secondFilterExpression = new ComparisonExpression("Age", ComparisonOperator.LessThan, 65);
        var secondFilterStatement = "Age lt 65";

        var parser = new Mock<IQueryCriteriaParser>();
        parser.Setup(_ => _.ParseFilterExpression(It.Is<string>(p => p == firstFilterStatement)))
            .Returns(firstFilterExpression);
        parser.Setup(_ => _.ParseFilterExpression(It.Is<string>(p => p == secondFilterStatement)))
            .Returns(secondFilterExpression);

        var toolkit = new Mock<IQueryToolkit>();
        toolkit.SetupGet(_ => _.Parser).Returns(parser.Object);

        var sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);

        var actual = sut
            .WithFilter(firstFilterStatement)
            .WithFilter(secondFilterStatement);

        var expectedFilterExpression = new LogicalExpression(LogicalOperator.And, firstFilterExpression, secondFilterExpression);
        var expectedFilterCriteria = new FilterCriteria(expectedFilterExpression);

        Assert.NotNull(actual);
        Assert.NotNull(actual.Criteria);
        Assert.NotNull(actual.Criteria.Filter);
        Assert.Equal(expectedFilterCriteria, actual.Criteria.Filter);

        parser.Verify(_ => _.ParseFilterExpression(It.IsAny<string>()), Times.Exactly(2));
    }
}