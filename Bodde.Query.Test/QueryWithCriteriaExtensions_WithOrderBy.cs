using Bodde.Query.Core;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Mocked;
using Bodde.Query.Test.Models;
using Moq;
using static Bodde.Query.Abstractions.Models.OrderByCriteria;

namespace Bodde.Query.Test;

public class QueryWithCriteriaExtensions_WithOrderBy
{
    private readonly QueryToolkitMock toolkit;
    private readonly QueryWithCriteria<Employee> sut;

    public QueryWithCriteriaExtensions_WithOrderBy()
    {
        toolkit = new QueryToolkitMock();
        sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);
    }

    [Fact]
    public void NullStatement_Throw()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => sut.WithOrderBy(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void ValidStatement()
    {
        var orderByStatement = "Salary desc";
        var orderByCriteria = new OrderByCriteria(new OrderByItem("Salary", SortDirection.Descending));

        toolkit.Parser
            .Setup(_ => _.ParseOrderBy(It.Is<string>(p => p == orderByStatement)))
            .Returns(orderByCriteria);

        var sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);

        var actual = sut.WithOrderBy(orderByStatement);

        Assert.NotNull(actual);
        Assert.NotNull(actual.Criteria);
        Assert.NotNull(actual.Criteria.OrderBy);
        Assert.Equal(orderByCriteria, actual.Criteria.OrderBy);

        toolkit.Parser.Verify(_ => _.ParseOrderBy(It.IsAny<string>()), Times.Once);
    }
}