using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class ODataFormatter_FormatFilter
{
    private readonly ODataFormatter sut;

    public ODataFormatter_FormatFilter()
    {
        sut = new ODataFormatter();
    }

    [Fact]
    public void SimpleEqualsFilter()
    {
        var filter = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Name",
                Operator: FilterCriteria.ComparisonOperator.Equals,
                Value: "John"
                )
            );

        var result = sut.FormatFilter(filter);

        Assert.Equal("$filter=Name eq 'John'", result);
    }

    [Fact]
    public void SimpleGreaterThanFilter()
    {
        var filter = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                    PropertyPath: "Age",
                    Operator: FilterCriteria.ComparisonOperator.GreaterThan,
                    Value: 30
                )
            );

        var result = sut.FormatFilter(filter);

        Assert.Equal("$filter=Age gt 30", result);
    }

    [Fact]
    public void SimpleContainsFilter()
    {
        var filter = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Description",
                Operator: FilterCriteria.ComparisonOperator.Contains,
                Value: "test"
                )
            );

        var result = sut.FormatFilter(filter);

        Assert.Equal("$filter=Description contains 'test'", result);
    }

    [Fact]
    public void InOperatorFilter()
    {
         var filter = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                    PropertyPath: "Id",
                    Operator: FilterCriteria.ComparisonOperator.In,
                    Value: new[] { 1, 2, 3 }
                )
            );

        var result = sut.FormatFilter(filter);

        Assert.Equal("$filter=Id in (1, 2, 3)", result);
    }

    [Fact]
    public void NullValueFilter()
    {
        var filter = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                    PropertyPath: "DeletedAt",
                    Operator: FilterCriteria.ComparisonOperator.Equals,
                    Value: null
                )
            );

        var result = sut.FormatFilter(filter);

        Assert.Equal("$filter=DeletedAt eq null", result);
    }

    [Fact]
    public void DateTimeValueFilter()
    {
        var dateTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var filter = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                    PropertyPath: "CreatedAt",
                    Operator: FilterCriteria.ComparisonOperator.GreaterThanOrEqual,
                    Value: dateTime
                )
            );

        var result = sut.FormatFilter(filter);

        Assert.Equal("$filter=CreatedAt ge 2024-01-01T12:00:00.0000000Z", result);
    }

    [Fact]
    public void Filter1_And_Filter2()
    {
        var filter = new FilterCriteria(
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

        var result = sut.FormatFilter(filter);

        Assert.Equal("$filter=Age gt 25 and Name startswith 'J'", result);
    }

    [Fact]
    public void Not_Filter()
    {
        var filter = new FilterCriteria(
            new FilterCriteria.NotExpression(
                new FilterCriteria.ComparisonExpression(
                    PropertyPath: "IsActive",
                    Operator: FilterCriteria.ComparisonOperator.Equals,
                    Value: true
                    )
                )
            );

        var result = sut.FormatFilter(filter);

        Assert.Equal("$filter=not (IsActive eq true)", result);
    }

    [Fact]
    public void Complex_Nested_Filter()
    {
        var filter = new FilterCriteria(
            new FilterCriteria.LogicalExpression(
                Operator: FilterCriteria.LogicalOperator.Or,
                First: new FilterCriteria.LogicalExpression(
                    Operator: FilterCriteria.LogicalOperator.And,
                    First: new FilterCriteria.ComparisonExpression(
                            PropertyPath: "Age",
                        Operator: FilterCriteria.ComparisonOperator.GreaterThan,
                        Value: 30
                        ),
                        Second: new FilterCriteria.ComparisonExpression(
                            PropertyPath: "Name",
                            Operator: FilterCriteria.ComparisonOperator.Contains,
                            Value: "Smith"
                        )
                    ),
                    Second: new FilterCriteria.NotExpression(
                        new FilterCriteria.ComparisonExpression(
                            PropertyPath: "IsActive",
                            Operator: FilterCriteria.ComparisonOperator.Equals,
                            Value: false
                        )
                    )
                )
            );
        var result = sut.FormatFilter(filter);
        Assert.Equal("$filter=Age gt 30 and Name contains 'Smith' or not (IsActive eq false)", result);
    }
}