using System.Globalization;
using Bodde.Query.Core;
using Bodde.Query.Test.Models;
using static Bodde.Query.Abstractions.Models.FilterCriteria;
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
        var filterExpression = new LogicalExpression(
            LOp.And,
            new ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 10000),
            null
        );

        Assert.Throws<InvalidOperationException>(() => sut.CreateFilterExpression<Employee>(filterExpression));
    }

    [Fact]
    public void ParameterType_Employee()
    {
        var filterExpression = new ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 10000);
        
        var actual = sut.CreateFilterExpression<Employee>(filterExpression);

        Assert.Equal(typeof(Employee), actual.Parameters.First().Type);
    }

    [Fact]
    public void ComparisonExpression_DateTime_ConvertedTo_DateTimeOffset()
    {
        var dateTime = DateTime.Parse("2024-01-01T12:00:00Z", provider: null, DateTimeStyles.RoundtripKind);

        var filterExpression = new ComparisonExpression(
            PropertyPath: nameof(Employee.HireDate),
            Operator: COp.GreaterThan,
            Value: dateTime
        );

        var actual = sut.CreateFilterExpression<Employee>(filterExpression);

        var binaryExpression = actual.Body as System.Linq.Expressions.BinaryExpression;
        Assert.NotNull(binaryExpression);

        Assert.Equal(typeof(DateTimeOffset), binaryExpression.Right.Type);
    }

    [Fact]
    public void ComparisonExpression_PropertyType_Long_In_Integer_Values()
    {
        int[] intValues = [0, 1];
        var filterExpression = new ComparisonExpression(
            PropertyPath: nameof(Employee.Id),  // property type is long
            Operator: COp.In,
            Value: intValues
        );

        var actual = sut.CreateFilterExpression<Employee>(filterExpression);

        var methodCallExpression = actual.Body as System.Linq.Expressions.MethodCallExpression;
        Assert.NotNull(methodCallExpression);   // in operator transformed into IEnumerable.Contains method

        var actualValues = methodCallExpression.Arguments.First();
        Assert.Equal(typeof(IEnumerable<long>), actualValues.Type);
    }

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    [Fact]
    public void Throws_OnUnsupportedFilterExpressionType()
    {
        var filterExpression = new UnsupportedFilterExpression();

        var exception = Assert.Throws<NotImplementedException>(() => sut.CreateFilterExpression<Employee>(filterExpression));
        
        Assert.Equal("Filter expression type UnsupportedFilterExpression is not implemented.", exception.Message);
    }

    record UnsupportedFilterExpression : FilterExpression
    {
    }

}
