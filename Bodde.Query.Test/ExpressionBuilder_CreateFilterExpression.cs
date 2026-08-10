using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Bodde.Query.Test.Models;
using COp = Bodde.Query.Abstractions.Models.FilterCriteria.ComparisonOperator;
using LOp = Bodde.Query.Abstractions.Models.FilterCriteria.LogicalOperator;

namespace Bodde.Query.Test;

public class ExpressionBuilder_CreateFilterExpression
{

    private readonly ExpressionBuilder sut = new();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

    [Fact]
    public void Throws_OnNullFilterExpression()
    {
        Assert.Throws<ArgumentNullException>(() => sut.CreateFilterExpression<Employee>(null));
    }

    [Fact]
    public void Throws_OnInvalidLogicalExpression_WithSingleExpression()
    {
        var filterExpression = new FilterCriteria.LogicalExpression(
            LOp.And,
            new FilterCriteria.ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 10000),
            null
        );

        Assert.Throws<InvalidOperationException>(() => sut.CreateFilterExpression<Employee>(filterExpression));
    }

    [Fact]
    public void ComparisonExpression_DateTime_ConvertedTo_DateTimeOffset()
    {
        var dateTime = DateTime.Parse("2024-01-01T12:00:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind);

        var filterExpression = new FilterCriteria.ComparisonExpression(
            PropertyPath: nameof(Employee.HireDate),
            Operator: COp.GreaterThan,
            Value: dateTime
        );

        var result = sut.CreateFilterExpression<Employee>(filterExpression);

        Assert.NotNull(result);
        var parameter = result.Parameters[0];
        Assert.Equal(typeof(Employee), parameter.Type);
    }

    [Fact]
    public void ComparisonExpression_PropertyType_Long_In_Integer_Values()
    {
        var value = new[] { 0, 1 };
        var filterExpression = new FilterCriteria.ComparisonExpression(
            PropertyPath: nameof(Employee.Id),
            Operator: COp.In,
            Value: value
        );

        var result = sut.CreateFilterExpression<Employee>(filterExpression);

        Assert.NotNull(result);
        var parameter = result.Parameters[0];
        Assert.Equal(typeof(Employee), parameter.Type);
    }

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    [Fact]
    public void Throws_OnUnsupportedFilterExpressionType()
    {
        var filterExpression = new UnsupportedFilterExpression();

        var exception = Assert.Throws<NotImplementedException>(() => sut.CreateFilterExpression<Employee>(filterExpression));
        
        Assert.Equal("Filter expression type UnsupportedFilterExpression is not implemented.", exception.Message);
    }

    record UnsupportedFilterExpression : FilterCriteria.FilterExpression
    {
    }

}
