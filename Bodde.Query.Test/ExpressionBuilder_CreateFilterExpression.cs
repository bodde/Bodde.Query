using System.Globalization;
using Bodde.Query.Core;
using Bodde.Query.Test.Models;
using static Bodde.Query.Abstractions.Models.FilterCriteria;
using static Bodde.Query.Test.Models.Employee;
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
    public void Throws_OnInvalidLogicalExpression_LeftExpressionMissing()
    {
        var filterExpression = new LogicalExpression(
            LOp.And,
            null,
            new ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 10000)
        );

        Assert.Throws<InvalidOperationException>(() => sut.CreateFilterExpression<Employee>(filterExpression));
    }

    [Fact]
    public void Throws_OnInvalidLogicalExpression_RightExpressionMissing()
    {
        var filterExpression = new LogicalExpression(
            LOp.And,
            new ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 10000),
            null
        );

        Assert.Throws<InvalidOperationException>(() => sut.CreateFilterExpression<Employee>(filterExpression));
    }

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    [Fact]
    public void ParameterType_Employee()
    {
        var filterExpression = new ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 10000);
        
        var actual = sut.CreateFilterExpression<Employee>(filterExpression);

        Assert.Equal(typeof(Employee), actual.Parameters.First().Type);
    }

    [Fact]
    public void ComparisonExpression_Property_DateTimeOffset_Value_DateTime()
    {
        var dateTime = DateTime.Parse("2024-01-01T12:00:00Z", provider: null, DateTimeStyles.RoundtripKind);

        var filterExpression = new ComparisonExpression(
            PropertyPath: nameof(Employee.HireDateTimeOffset),
            Operator: COp.GreaterThan,
            Value: dateTime
        );

        var actual = sut.CreateFilterExpression<Employee>(filterExpression);

        var binaryExpression = actual.Body as System.Linq.Expressions.BinaryExpression;
        Assert.NotNull(binaryExpression);

        Assert.Equal(typeof(DateTimeOffset), binaryExpression.Right.Type);
    }

    [Fact]
    public void ComparisonExpression_Property_DateTime_Value_Null()
    {
        var filterExpression = new ComparisonExpression(
            PropertyPath: nameof(Employee.Email),
            Operator: COp.Equals,
            Value: null
        );

        var actual = sut.CreateFilterExpression<Employee>(filterExpression);

        var binaryExpression = actual.Body as System.Linq.Expressions.BinaryExpression;
        Assert.NotNull(binaryExpression);
        Assert.Equal("null", binaryExpression.Right.ToString());
    }

    [Fact]
    public void ComparisonExpression_Property_DateTime_Value_DateTimeOffset()
    {
        var dateTime = DateTimeOffset.Parse("2024-01-01T12:00:00Z");

        var filterExpression = new ComparisonExpression(
            PropertyPath: nameof(Employee.HireDate),
            Operator: COp.GreaterThan,
            Value: dateTime
        );

        var actual = sut.CreateFilterExpression<Employee>(filterExpression);

        var binaryExpression = actual.Body as System.Linq.Expressions.BinaryExpression;
        Assert.NotNull(binaryExpression);

        Assert.Equal(typeof(DateTime), binaryExpression.Right.Type);
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

    [Theory]
    [InlineData(COp.StartsWith)]
    [InlineData(COp.EndsWith)]
    [InlineData(COp.Contains)]
    public void Throws_StringOperator_Applied_To_NonString_Property(COp stringOperator)
    {
        var filterExpression = new ComparisonExpression(
            PropertyPath: nameof(Employee.Id),  // property type is long
            Operator: stringOperator,
            Value: 2
        );

        var exception = Assert.Throws<InvalidOperationException>(() => sut.CreateFilterExpression<Employee>(filterExpression));  

        Assert.Equal($"Operator {stringOperator} can only be applied to string properties.", exception.Message);
    }

    [Theory]
    [InlineData(1)]
    [InlineData("x")]
    public void Throws_In_Operator_Value_Not_Collection(object notACollection)
    {
        var filterExpression = new ComparisonExpression(
            PropertyPath: nameof(Employee.Id),  // property type is long
            Operator: COp.In,
            Value: notACollection
        );

        var exception = Assert.Throws<InvalidOperationException>(() => sut.CreateFilterExpression<Employee>(filterExpression));  

        Assert.Equal($"Value for 'In' operator must be a collection.", exception.Message);
    }

    [Fact]
    public void Throws_In_Operator_Value_Generics_Not_Collection()
    {
        var filterExpression = new ComparisonExpression(
            PropertyPath: nameof(Employee.Id),  // property type is long
            Operator: COp.In,
            Value: new KeyValuePair<string, string>("x", "y")
        );

        var exception = Assert.Throws<InvalidOperationException>(() => sut.CreateFilterExpression<Employee>(filterExpression));  

        Assert.Equal($"Value for 'In' operator must be a collection.", exception.Message);
    }

    [Fact]
    public void Throws_FilterExpressionType_NotImplemented()
    {
        var filterExpression = new NotImplementedFilterExpression();

        var exception = Assert.Throws<NotImplementedException>(() => sut.CreateFilterExpression<Employee>(filterExpression));
        
        Assert.Equal("Filter expression type NotImplementedFilterExpression is not implemented.", exception.Message);
    }


    [Fact]
    public void Throws_LogicalOperator_NotImplemented()
    {        
        LOp logicalOperator = (LOp)100;
        var filterExpression = new LogicalExpression(
            logicalOperator,
            new ComparisonExpression(nameof(Employee.Role), COp.Equals, RoleType.Manager),
            new ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 10000)
        );

        var exception = Assert.Throws<NotImplementedException>(() => sut.CreateFilterExpression<Employee>(filterExpression));
        
        Assert.Equal($"Logical operator {logicalOperator} is not implemented.", exception.Message);
    }


    [Fact]
    public void Throws_ComparisonOperator_NotImplemented()
    {        
        COp comparisonOperator = (COp)100;
        var comparisonExpression =  new ComparisonExpression(
            PropertyPath: nameof(Employee.Id),
            Operator: comparisonOperator,
            Value: 2
        );

        var exception = Assert.Throws<NotImplementedException>(() => sut.CreateFilterExpression<Employee>(comparisonExpression));
        
        Assert.Equal($"Operator {comparisonOperator} is not implemented.", exception.Message);
    }

    record NotImplementedFilterExpression : FilterExpression
    {
    }

}
