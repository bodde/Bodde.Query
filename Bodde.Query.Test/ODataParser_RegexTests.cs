using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Xunit;

namespace Bodde.Query.Test;

public class ODataParser_RegexTests
{
    [Fact]
    public void GetComparisonStatements_InOperator_ReturnsSingleStatement()
    {
        var sut = new ODataParser();

        var input = "Id in (1, 2, 3)";
        var statements = sut.GetComparisonStatements(input);

        Assert.Single(statements);
        Assert.Equal("Id in (1, 2, 3)", statements[0]);
    }

    [Fact]
    public void GetComparisonStatements_MultipleStatements_ReturnsAll()
    {
        var sut = new ODataParser();

        var input = "Name eq 'John' and Age gt 30 or Description contains 'x'";
        var statements = sut.GetComparisonStatements(input);

        Assert.Equal(3, statements.Length);
        Assert.Contains("Name eq 'John'", statements);
        Assert.Contains("Age gt 30", statements);
        Assert.Contains("Description contains 'x'", statements);
    }

    [Fact]
    public void ParseValue_VariousTypes_ParsesCorrectly()
    {
        var (nullValue, nullType) = ODataParser.ParseValue("null");
        Assert.Null(nullValue);
        Assert.Equal(typeof(object), nullType);

        var (stringValue, stringType) = ODataParser.ParseValue("'test'");
        Assert.Equal("test", stringValue);
        Assert.Equal(typeof(string), stringType);

        var (intValue, intType) = ODataParser.ParseValue("42");
        Assert.Equal(42, intValue);
        Assert.Equal(typeof(int), intType);

        var (doubleValue, doubleType) = ODataParser.ParseValue("3.14");
        Assert.IsType<double>(doubleValue);
        Assert.Equal(3.14d, (double)doubleValue);
        Assert.Equal(typeof(double), doubleType);

        var (boolValue, boolType) = ODataParser.ParseValue("true");
        Assert.Equal(true, boolValue);
        Assert.Equal(typeof(bool), boolType);

        var (dateValue, dateType) = ODataParser.ParseValue("2024-01-01T12:00:00Z");
        var expected = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        Assert.IsType<DateTime>(dateValue);
        Assert.Equal(expected, (DateTime)dateValue);
        Assert.Equal(typeof(DateTime), dateType);
    }

    [Fact]
    public void ParsePaging_SkipTopCount_ParsesCorrectly()
    {
        var sut = new ODataParser();

        var result = sut.ParsePaging("$skip=5&$top=10&$count=true");
        Assert.NotNull(result);
        Assert.Equal(5, result!.Skip);
        Assert.Equal(10, result.Top);
        Assert.True(result.TotalCount.HasValue && result.TotalCount.Value);

        var result2 = sut.ParsePaging("something=else&$top=3");
        Assert.NotNull(result2);
        Assert.Null(result2!.Skip);
        Assert.Equal(3, result2.Top);
        Assert.Null(result2.TotalCount);
    }

    [Fact]
    public void ParseOrderBy_MultipleItems_ParsesDirections()
    {
        var sut = new ODataParser();

        var result = sut.ParseOrderBy("$orderby=Name desc,Age");
        Assert.NotNull(result);
        Assert.Equal(2, result!.Items.Length);
        Assert.Equal("Name", result.Items[0].PropertyPath);
        Assert.Equal(OrderByCriteria.SortDirection.Descending, result.Items[0].Direction);
        Assert.Equal("Age", result.Items[1].PropertyPath);
        Assert.Equal(OrderByCriteria.SortDirection.Ascending, result.Items[1].Direction);
    }
}
