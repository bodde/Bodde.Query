using Bodde.Query.Abstractions.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;
using Bodde.Query.Core;
using Bodde.Query.Test.Helpers;
using Moq;
using static Bodde.Query.Abstractions.Models.OrderByCriteria;

namespace Bodde.Query.Test;

public class QueryableWithCriteriaExtensions_WithOrderBy
{
    [Fact]
    public void NullStatement_Throw()
    {
        var toolkit = new Mock<IQueryToolkit>();

        var sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => sut.WithOrderBy(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public void ValidStatement()
    {
        var orderByStatement = "Salary desc";
        var orderByCriteria = new OrderByCriteria(new OrderByItem("Salary", SortDirection.Descending));

        var parser = new Mock<IQueryCriteriaParser>();
        parser.Setup(_ => _.ParseOrderBy(It.Is<string>(p => p == orderByStatement)))
            .Returns(orderByCriteria);

        var toolkit = new Mock<IQueryToolkit>();
        toolkit.SetupGet(_ => _.Parser).Returns(parser.Object);

        var sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);

        var actual = sut.WithOrderBy(orderByStatement);

        Assert.NotNull(actual);
        Assert.NotNull(actual.Criteria);
        Assert.NotNull(actual.Criteria.OrderBy);
        Assert.Equal(orderByCriteria, actual.Criteria.OrderBy);

        parser.Verify(_ => _.ParseOrderBy(It.IsAny<string>()), Times.Once);
    }
}