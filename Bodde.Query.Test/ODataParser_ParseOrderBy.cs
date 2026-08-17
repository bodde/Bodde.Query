using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class ODataParser_ParseOrderBy
{
    private readonly ODataParser sut;

    public ODataParser_ParseOrderBy()
    {
        sut = new ODataParser();
    }

    [Theory]
    [InlineData("")]
    [InlineData("$orderby=")]
    public void EmptyString_EmptyCriteria(string input)
    {
        var result = sut.ParseOrderBy(input);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public void Prefixed_SingleAscendingOrderBy()
    {
        var input = "$orderby=Name asc";

        var result = sut.ParseOrderBy(input);

        var expected = new OrderByCriteria(
            new OrderByCriteria.OrderByItem(
                PropertyPath: "Name",
                Direction: OrderByCriteria.SortDirection.Ascending
            )
        );

        Assert.Equal(expected.Items[0], result!.Items[0]);
    }

    [Fact]
    public void SingleDescendingOrderBy()
    {
        var input = "CreatedDate desc";

        var result = sut.ParseOrderBy(input);

        var expected = new OrderByCriteria(
            new OrderByCriteria.OrderByItem(
                PropertyPath: "CreatedDate",
                Direction: OrderByCriteria.SortDirection.Descending
            )
        );

        Assert.Equal(expected.Items[0], result!.Items[0]);
    }

    [Fact]
    public void MultipleOrderByItems()
    {
        var input = "LastName asc, FirstName desc, Age asc";

        var result = sut.ParseOrderBy(input);

        var expected = new OrderByCriteria(
            new OrderByCriteria.OrderByItem(
                PropertyPath: "LastName",
                Direction: OrderByCriteria.SortDirection.Ascending
            ),
            new OrderByCriteria.OrderByItem(
                PropertyPath: "FirstName",
                Direction: OrderByCriteria.SortDirection.Descending
            ),
            new OrderByCriteria.OrderByItem(
                PropertyPath: "Age",
                Direction: OrderByCriteria.SortDirection.Ascending
            )
        );

        Assert.Equal(expected.Items[0], result!.Items[0]);
        Assert.Equal(expected.Items[1], result!.Items[1]);
        Assert.Equal(expected.Items[2], result!.Items[2]);
    }
}
