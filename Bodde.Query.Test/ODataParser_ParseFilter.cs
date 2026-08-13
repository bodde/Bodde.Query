using Bodde.Common.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class ODataParser_ParseFilter
{
    private readonly ODataParser sut;

    public ODataParser_ParseFilter()
    {
        sut = new ODataParser();
    }

    [Theory]
    [InlineData("")]
    [InlineData("$filter=")]
    public void EmptyString_Throw_FormatException(string input)
    {
        Assert.Throws<FormatException>(() => sut.ParseFilter(input));
    }

    [Fact]
    public void Prefixed_SimpleEqualsFilter()
    {
        var input = "$filter=Name eq 'John'";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Name",
                Operator: FilterCriteria.ComparisonOperator.Equals,
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
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Name",
                Operator: FilterCriteria.ComparisonOperator.Equals,
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
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Name",
                Operator: FilterCriteria.ComparisonOperator.Equals,
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
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Age",
                Operator: FilterCriteria.ComparisonOperator.GreaterThan,
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
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Description",
                Operator: FilterCriteria.ComparisonOperator.Contains,
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
        var expected = new FilterCriteria.ComparisonExpression(
                PropertyPath: "Id",
                Operator: FilterCriteria.ComparisonOperator.In,
                Value: expectedValues
            );  

        var actual = result?.Expression as FilterCriteria.ComparisonExpression;
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
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "DeletedAt",
                Operator: FilterCriteria.ComparisonOperator.Equals,
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
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "CreatedAt",
                Operator: FilterCriteria.ComparisonOperator.GreaterThanOrEqual,
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
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "IsActive",
                Operator: FilterCriteria.ComparisonOperator.Equals,
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
            new FilterCriteria.LogicalExpression(
                Operator: FilterCriteria.LogicalOperator.And,
                First: new FilterCriteria.ComparisonExpression(
                    PropertyPath: "Age",
                    Operator: FilterCriteria.ComparisonOperator.GreaterThan,
                    Value: 25
                ),
                Second: new FilterCriteria.ComparisonExpression(
                    PropertyPath: "Name",
                    Operator: FilterCriteria.ComparisonOperator.StartsWith,
                    Value: "J"
                )
            )
        );

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Not_Filter()
    {
        var input = "not (IsActive eq true)";

        var result = sut.ParseFilter(input);

        var expected = new FilterCriteria(
            new FilterCriteria.NotExpression(
                new FilterCriteria.ComparisonExpression(
                    PropertyPath: "IsActive",
                    Operator: FilterCriteria.ComparisonOperator.Equals,
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
            new FilterCriteria.NotExpression(
                new FilterCriteria.LogicalExpression(
                    Operator: FilterCriteria.LogicalOperator.And,
                    First: new FilterCriteria.ComparisonExpression(
                        PropertyPath: "IsActive",
                        Operator: FilterCriteria.ComparisonOperator.Equals,
                        Value: true
                    ),
                    Second: new FilterCriteria.ComparisonExpression(
                        PropertyPath: "Age",
                        Operator: FilterCriteria.ComparisonOperator.LessThan,
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
            new FilterCriteria.LogicalExpression(
                Operator: FilterCriteria.LogicalOperator.And,
                First: new FilterCriteria.ComparisonExpression(
                    PropertyPath: "Age",
                    Operator: FilterCriteria.ComparisonOperator.GreaterThan,
                    Value: 30
                ),
                Second: new FilterCriteria.LogicalExpression(
                    Operator: FilterCriteria.LogicalOperator.Or,
                    First: new FilterCriteria.ComparisonExpression(
                        PropertyPath: "Name",
                        Operator: FilterCriteria.ComparisonOperator.Contains,
                        Value: "Smith"
                    ),
                    Second: new FilterCriteria.NotExpression(
                        new FilterCriteria.ComparisonExpression(
                            PropertyPath: "IsActive",
                            Operator: FilterCriteria.ComparisonOperator.Equals,
                            Value: false
                        )
                    )
                )
            )
        );

        Assert.Equal(expected, result);
    }
}
