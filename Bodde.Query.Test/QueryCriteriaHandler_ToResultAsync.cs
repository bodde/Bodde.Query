using Moq;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Models;
using Bodde.Query.Test.Extensions;
using Bodde.Query.Abstractions.Services;
using COp = Bodde.Query.Abstractions.Models.FilterCriteria.ComparisonOperator;

namespace Bodde.Query.Test;

public class QueryCriteriaHandler_ToResultAsync
{
    private readonly Mock<IQueryExecutor> queryExecutor;
    private readonly QueryCriteriaHandler sut;

    public QueryCriteriaHandler_ToResultAsync()
    {
        queryExecutor = new Mock<IQueryExecutor>();

        queryExecutor.Setup(q => q
            .ToArrayAsync(It.IsAny<IQueryable<Employee>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Employee> q, CancellationToken ct) => q.ToArray()
            );
        queryExecutor.Setup(q => q
            .CountAsync(It.IsAny<IQueryable<Employee>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<Employee> q, CancellationToken ct) => q.Count()
            );
        
        sut = new QueryCriteriaHandler(new ExpressionBuilder(), queryExecutor.Object);
    }


    [Fact]
    public async Task ToResultAsync_IQueryable_WithTotalCount()
    {
        var filterExpression = new FilterCriteria.ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 80000);
        var filterCriteria = new FilterCriteria(filterExpression);
        var orderByCriteria = new OrderByCriteria(new OrderByCriteria.OrderByItem(nameof(Employee.LastName)));
        var pagingCriteria = new PagingCriteria(1, 2, TotalCount: true);
        var fullQueryCriteria = new QueryCriteria(filterCriteria, orderByCriteria, pagingCriteria);
        var filteredQueryCriteria = new QueryCriteria(filterCriteria);

        var data = EmployeeSetBuilder.Build().AsQueryable();
        var expectedData = sut.ApplyCriteria(data, fullQueryCriteria).ToArray();
        var expectedTotalCount = sut.ApplyCriteria(data, filteredQueryCriteria).Count();

        var actual = await sut.ToResultAsync(data, fullQueryCriteria);

        Assert.Equal(expectedData.GetIdsCsv(), actual.Items.GetIdsCsv());
        Assert.Equal(expectedTotalCount, actual.TotalCount);

        queryExecutor.Verify(q => q.ToArrayAsync(It.IsAny<IQueryable<Employee>>(), It.IsAny<CancellationToken>()), Times.Once);
        queryExecutor.Verify(q => q.CountAsync(It.IsAny<IQueryable<Employee>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToResultAsync_IQueryable_WithoutTotalCount()
    {
        var filterExpression = new FilterCriteria.ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 80000);
        var filterCriteria = new FilterCriteria(filterExpression);
        var orderByCriteria = new OrderByCriteria(new OrderByCriteria.OrderByItem(nameof(Employee.LastName)));
        var pagingCriteria = new PagingCriteria(1, 2, TotalCount: false);
        var fullQueryCriteria = new QueryCriteria(filterCriteria, orderByCriteria, pagingCriteria);

        var data = EmployeeSetBuilder.Build().AsQueryable();
        var expectedData = sut.ApplyCriteria(data, fullQueryCriteria).ToArray();

        var actual = await sut.ToResultAsync(data, fullQueryCriteria);

        Assert.Equal(expectedData.GetIdsCsv(), actual.Items.GetIdsCsv());
        Assert.Null(actual.TotalCount);

        queryExecutor.Verify(q => q.ToArrayAsync(It.IsAny<IQueryable<Employee>>(), It.IsAny<CancellationToken>()), Times.Once);
        queryExecutor.Verify(q => q.CountAsync(It.IsAny<IQueryable<Employee>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ToResultAsync_QueryCriteriaQueryable_WithTotalCount()
    {
        var filterExpression = new FilterCriteria.ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 80000);
        var filterCriteria = new FilterCriteria(filterExpression);
        var orderByCriteria = new OrderByCriteria(new OrderByCriteria.OrderByItem(nameof(Employee.LastName)));
        var pagingCriteria = new PagingCriteria(1, 2, TotalCount: true);
        var fullQueryCriteria = new QueryCriteria(filterCriteria, orderByCriteria, pagingCriteria);
        var filteredQueryCriteria = new QueryCriteria(filterCriteria);

        var data = EmployeeSetBuilder.Build().AsQueryable();
        var criteriaQueryable = sut.ApplyCriteria(data, fullQueryCriteria);

        var expectedData = criteriaQueryable.ToArray();
        var expectedTotalCount = sut.ApplyCriteria(data, filteredQueryCriteria).Count();

        var actual = await sut.ToResultAsync(criteriaQueryable);

        Assert.Equal(expectedData.GetIdsCsv(), actual.Items.GetIdsCsv());
        Assert.Equal(expectedTotalCount, actual.TotalCount);

        queryExecutor.Verify(q => q.ToArrayAsync(It.IsAny<IQueryable<Employee>>(), It.IsAny<CancellationToken>()), Times.Once);
        queryExecutor.Verify(q => q.CountAsync(It.IsAny<IQueryable<Employee>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToResultAsync_QueryCriteriaQueryable_WithoutTotalCount()
    {
        var filterExpression = new FilterCriteria.ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, 80000);
        var filterCriteria = new FilterCriteria(filterExpression);
        var orderByCriteria = new OrderByCriteria(new OrderByCriteria.OrderByItem(nameof(Employee.LastName)));
        var pagingCriteria = new PagingCriteria(1, 2, TotalCount: false);
        var fullQueryCriteria = new QueryCriteria(filterCriteria, orderByCriteria, pagingCriteria);

        var data = EmployeeSetBuilder.Build().AsQueryable();
        var criteriaQueryable = sut.ApplyCriteria(data, fullQueryCriteria);
        var expectedData = criteriaQueryable.ToArray();

        var actual = await sut.ToResultAsync(criteriaQueryable);

        Assert.Equal(expectedData.GetIdsCsv(), actual.Items.GetIdsCsv());
        Assert.Null(actual.TotalCount);

        queryExecutor.Verify(q => q.ToArrayAsync(It.IsAny<IQueryable<Employee>>(), It.IsAny<CancellationToken>()), Times.Once);
        queryExecutor.Verify(q => q.CountAsync(It.IsAny<IQueryable<Employee>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
