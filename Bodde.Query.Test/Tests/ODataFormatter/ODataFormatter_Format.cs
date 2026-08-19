using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class ODataFormatter_Format
{
    private readonly ODataFormatter sut;

    public ODataFormatter_Format()
    {
        sut = new ODataFormatter();
    }

    [Fact]
    public void Criteria_Paging()
    {
        var paging = new PagingCriteria(Skip: 5, Top: 20, TotalCount: true);
        var criteria = new QueryCriteria(Paging: paging);

        var result = sut.Format(criteria);

        Assert.Equal("$skip=5&$top=20&$count=true", result);
    }

    [Fact]
    public void Criteria_Filter()
    {
        var filter = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Name",
                Operator: FilterCriteria.ComparisonOperator.Equals,
                Value: "John"
            )
        );
        var criteria = new QueryCriteria(Filter: filter);
        var result = sut.Format(criteria);
        Assert.Equal("$filter=Name eq 'John'", result);
    }

    [Fact]
    public void Criteria_Filter_Paging()
    {
        var filter = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Name",
                Operator: FilterCriteria.ComparisonOperator.Equals,
                Value: "John"
            )
        );
        var paging = new PagingCriteria(Skip: 5, Top: 20, TotalCount: true);
        var criteria = new QueryCriteria(Filter: filter, Paging: paging);

        var result = sut.Format(criteria);

        Assert.Equal("$skip=5&$top=20&$count=true&$filter=Name eq 'John'", result);
    }

    [Fact]
    public void Criteria_OrderBy()
    {
        var orderBy = new OrderByCriteria(
        [
            new OrderByCriteria.OrderByItem(
                PropertyPath: "Name",
                Direction: OrderByCriteria.SortDirection.Ascending
            )
        ]);
        var criteria = new QueryCriteria(OrderBy: orderBy);

        var result = sut.Format(criteria);

        Assert.Equal("$orderby=Name asc", result);
    }

    [Fact]
    public void Criteria_Filter_OrderBy()
    {
        var filter = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Name",
                Operator: FilterCriteria.ComparisonOperator.Equals,
                Value: "John"
            )
        );
        var orderBy = new OrderByCriteria(
        [
            new OrderByCriteria.OrderByItem(
                PropertyPath: "Name",
                Direction: OrderByCriteria.SortDirection.Ascending
            )
        ]);
        var criteria = new QueryCriteria(Filter: filter, OrderBy: orderBy);

        var result = sut.Format(criteria);

        Assert.Equal("$filter=Name eq 'John'&$orderby=Name asc", result);
    }

    [Fact]
    public void Criteria_Paging_OrderBy()
    {
        var paging = new PagingCriteria(Skip: 5, Top: 20, TotalCount: true);
        var orderBy = new OrderByCriteria(
        [
            new OrderByCriteria.OrderByItem(
                PropertyPath: "Name",
                Direction: OrderByCriteria.SortDirection.Ascending
            )
        ]);
        var criteria = new QueryCriteria(Paging: paging, OrderBy: orderBy);

        var result = sut.Format(criteria);

        Assert.Equal("$skip=5&$top=20&$count=true&$orderby=Name asc", result);
    }

    [Fact]
    public void Criteria_AllParts()
    {
        var filter = new FilterCriteria(
            new FilterCriteria.ComparisonExpression(
                PropertyPath: "Name",
                Operator: FilterCriteria.ComparisonOperator.Equals,
                Value: "John"
            )
        );
        var paging = new PagingCriteria(Skip: 5, Top: 20, TotalCount: true);
        var orderBy = new OrderByCriteria(
        [
            new OrderByCriteria.OrderByItem(
                PropertyPath: "Name",
                Direction: OrderByCriteria.SortDirection.Ascending
            )
        ]);
        var criteria = new QueryCriteria(Filter: filter, Paging: paging, OrderBy: orderBy);

        var result = sut.Format(criteria);

        Assert.Equal("$skip=5&$top=20&$count=true&$filter=Name eq 'John'&$orderby=Name asc", result);
    }
}