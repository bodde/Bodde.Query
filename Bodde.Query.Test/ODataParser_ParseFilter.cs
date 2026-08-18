using System.Reflection;
using Bodde.Common.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using static Bodde.Query.Abstractions.Models.FilterCriteria;

namespace Bodde.Query.Test;

public class ODataParser_ParseFilter
{
    private readonly ODataParser sut;

    public ODataParser_ParseFilter()
    {
        sut = new ODataParser();
    }


#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    [Fact]
    public void Null_Throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => sut.ParseFilter(null));
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    private const string NoComparisonStatements = "No comparison statements found in filter string.";

    [Theory]
    [InlineData("x", NoComparisonStatements)]
    [InlineData("x eq", NoComparisonStatements)]
    [InlineData("x in ()", NoComparisonStatements)]
    [InlineData("x in 1", "Invalid syntax for 'in' operator.")]
    [InlineData("x in (1,'a')", "All values for 'in' operator must be of the same type.")]
    [InlineData("x off 'a'", "OData operator 'off' is not supported. Supported operators are: eq,ne,gt,ge,lt,le,contains,startswith,endswith,in")]
    [InlineData("x gt 1 and y startswith 'J' or z eq 5", "Only one logical operator per logical expression is supported.")]
    public void BadFormatInput_Throw_FormatException(string input, string expectedMessage)
    {
        var ex = Assert.Throws<FormatException>(() => sut.ParseFilter(input));
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public void Prefixed_SimpleEqualsFilter()
    {
        var input = "$filter=Name eq 'John'";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new ComparisonExpression(
                PropertyPath: "Name",
                Operator: ComparisonOperator.Equals,
                Value: "John"
            )
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SimpleEqualsFilter()
    {
        var input = "Name eq 'John'";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new ComparisonExpression(
                PropertyPath: "Name",
                Operator: ComparisonOperator.Equals,
                Value: "John"
            )
        );

        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void SimpleEqualsNull()
    {
        var input = "Name eq null";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new ComparisonExpression(
                PropertyPath: "Name",
                Operator: ComparisonOperator.Equals,
                Value: null
            )
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public void SimpleGreaterThanFilter()
    {
        var input = "Age gt 30";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new ComparisonExpression(
                PropertyPath: "Age",
                Operator: ComparisonOperator.GreaterThan,
                Value: 30
            )
        );

        Assert.Equal(expected, result);
    }



    [Fact]
    public void SimpleContainsFilter()
    {
        var input = "Description contains 'test'";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new ComparisonExpression(
                PropertyPath: "Description",
                Operator: ComparisonOperator.Contains,
                Value: "test"
            )
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public void InOperatorFilter()
    {
        var input = "Id in (1, 2, 3)";

        var result = sut.ParseFilter(input);

        var expectedValues = new[] { 1, 2, 3 };
        var expected = new ComparisonExpression(
                PropertyPath: "Id",
                Operator: ComparisonOperator.In,
                Value: expectedValues
            );  

        var actual = result?.Expression as ComparisonExpression;
        Assert.NotNull(actual);
        Assert.Equal(expected.PropertyPath, actual!.PropertyPath);
        Assert.Equal(expected.Operator, actual.Operator);

        var actualValues = actual.Value as IEnumerable<int>;
        Assert.NotNull(actualValues);
        Assert.Equal(expectedValues.ToCsv(), actualValues.ToCsv());
    }

    [Fact]
    public void NullValueFilter()
    {
        var input = "DeletedAt eq null";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new ComparisonExpression(
                PropertyPath: "DeletedAt",
                Operator: ComparisonOperator.Equals,
                Value: null
            )
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public void DateTimeValueFilter()
    {
        var input = "CreatedAt ge 2024-01-01T12:00:00Z";
        var dateTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new ComparisonExpression(
                PropertyPath: "CreatedAt",
                Operator: ComparisonOperator.GreaterThanOrEqual,
                Value: dateTime
            )
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public void BooleanValueFilter()
    {
        var input = "IsActive eq true";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new ComparisonExpression(
                PropertyPath: "IsActive",
                Operator: ComparisonOperator.Equals,
                Value: true
            )
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Filter1_And_Filter2()
    {
        var input = "Age gt 25 and Name startswith 'J'";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new LogicalExpression(
                Operator: LogicalOperator.And,
                First: new ComparisonExpression(
                    PropertyPath: "Age",
                    Operator: ComparisonOperator.GreaterThan,
                    Value: 25
                ),
                Second: new ComparisonExpression(
                    PropertyPath: "Name",
                    Operator: ComparisonOperator.StartsWith,
                    Value: "J"
                )
            )
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Filter1_And_Filter2_And_Filter3()
    {
        var input = "Age gt 25 and Name startswith 'J' and Role eq 'Developer'";

        var result = sut.ParseFilter(input);

        var expectedLogicalExpression = new LogicalExpression(
                Operator: LogicalOperator.And,
                new ComparisonExpression(
                    PropertyPath: "Age",
                    Operator: ComparisonOperator.GreaterThan,
                    Value: 25
                ),
                new ComparisonExpression(
                    PropertyPath: "Name",
                    Operator: ComparisonOperator.StartsWith,
                    Value: "J"
                ),        
                new ComparisonExpression(
                    PropertyPath: "Role",
                    Operator: ComparisonOperator.Equals,
                    Value: "Developer"
                )
            );

        var actualLogicalExpression = result.Expression as LogicalExpression;

        Assert.NotNull(actualLogicalExpression);
        Assert.Equal(expectedLogicalExpression.AllExpressions, actualLogicalExpression.AllExpressions);
    }

    [Theory]
    [InlineData("not IsActive eq true")]
    [InlineData("not (IsActive eq true)")]
    public void Not_Filter(string input)
    {
        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new NotExpression(
                new ComparisonExpression(
                    PropertyPath: "IsActive",
                    Operator: ComparisonOperator.Equals,
                    Value: true
                )
            )
        );

        Assert.Equal(expected, result);
    }


    [Fact]
    public void Not_Complex_Filter()
    {
        var input = "not (IsActive eq true and Age lt 18)";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new NotExpression(
                new LogicalExpression(
                    Operator: LogicalOperator.And,
                    First: new ComparisonExpression(
                        PropertyPath: "IsActive",
                        Operator: ComparisonOperator.Equals,
                        Value: true
                    ),
                    Second: new ComparisonExpression(
                        PropertyPath: "Age",
                        Operator: ComparisonOperator.LessThan,
                        Value: 18
                    )
                )
            )
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Complex_Nested_Filter()
    {
        var input = "Age gt 30 and (Name contains 'Smith' or not (IsActive eq false))";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new LogicalExpression(
                Operator: LogicalOperator.And,
                First: new ComparisonExpression(
                    PropertyPath: "Age",
                    Operator: ComparisonOperator.GreaterThan,
                    Value: 30
                ),
                Second: new LogicalExpression(
                    Operator: LogicalOperator.Or,
                    First: new ComparisonExpression(
                        PropertyPath: "Name",
                        Operator: ComparisonOperator.Contains,
                        Value: "Smith"
                    ),
                    Second: new NotExpression(
                        new ComparisonExpression(
                            PropertyPath: "IsActive",
                            Operator: ComparisonOperator.Equals,
                            Value: false
                        )
                    )
                )
            )
        );

        Assert.Equal(expected, result);
    }
}
