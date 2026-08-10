using System;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class ODataFormatter_FormatOrderBy
{
    private readonly ODataFormatter sut;

    public ODataFormatter_FormatOrderBy()
    {
        sut = new ODataFormatter();
    }

    [Fact]
    public void OrderByCriteria_SingleAscending()
    {
        var orderBy = new OrderByCriteria(
        [
            new OrderByCriteria.OrderByItem(
                PropertyPath: "Name",
                Direction: OrderByCriteria.SortDirection.Ascending
            )
        ]);

        var result = sut.FormatOrderBy(orderBy);
        Assert.Equal("$orderby=Name asc", result);
    }

    [Fact]
    public void OrderByCriteria_MultipleMixed()
    {
        var orderBy = new OrderByCriteria(
        [
            new OrderByCriteria.OrderByItem(
                PropertyPath: "Age",
                Direction: OrderByCriteria.SortDirection.Descending
            ),
            new OrderByCriteria.OrderByItem(
                PropertyPath: "Name",
                Direction: OrderByCriteria.SortDirection.Ascending
            )
        ]);
        var result = sut.FormatOrderBy(orderBy);
        Assert.Equal("$orderby=Age desc,Name asc", result);
    }

    [Fact]
    public void OrderByCriteria_Empty()
    {
        var orderBy = new OrderByCriteria([]);

        var result = sut.FormatOrderBy(orderBy);
        Assert.Equal("", result);
    }
}
