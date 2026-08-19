using Bodde.Query.Core;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Mocked;
using Bodde.Query.Test.Models;
using Moq;

namespace Bodde.Query.Test;

public class QueryWithCriteriaExtensions_ForTotalCount
{
    private readonly QueryToolkitMock toolkit;
    private readonly QueryWithCriteria<Employee> sut;

    public QueryWithCriteriaExtensions_ForTotalCount()
    {
        toolkit = new QueryToolkitMock();
        sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);
    }
    
    [Fact]
    public void Test()
    {
        var filterStatement = "Salary gt 80000";
        var filterExpression = QueryCriteriaItemBuilder.SalaryGreaterThan80000;

        var orderByStatement = "LastName";
        var orderByCriteria = new OrderByCriteria(QueryCriteriaItemBuilder.OrderByLastName);
        
        toolkit.Parser
            .Setup(_ => _.ParseFilterExpression(It.Is<string>(p => p == filterStatement)))
            .Returns(filterExpression);

        toolkit.Parser
            .Setup(_ => _.ParseOrderBy(It.Is<string>(p => p == orderByStatement)))
            .Returns(orderByCriteria);

        var actual = sut
            .WithFilter(filterStatement)
            .WithOrderBy(orderByStatement)
            .WithPaging(skip: 20, top: 10, totalCount: true);

        var actualForCount = actual.ForTotalCount();  

        Assert.NotNull(actual);
        Assert.NotNull(actual.Criteria);
        Assert.NotNull(actual.Criteria.Filter);
        Assert.NotNull(actual.Criteria.OrderBy);
        Assert.NotNull(actual.Criteria.Paging);

        Assert.NotNull(actualForCount);
        Assert.NotNull(actualForCount.Criteria);
        Assert.NotNull(actualForCount.Criteria.Filter);
        Assert.Equal(filterExpression, actualForCount.Criteria.Filter.Expression);
        Assert.Null(actualForCount.Criteria.OrderBy);
        Assert.Null(actualForCount.Criteria.Paging);

    }

}