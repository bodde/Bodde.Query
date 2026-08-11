using Bodde.Query.Abstractions.Services;
using Bodde.Query.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Bodde.Query.Test;

public class ServiceCollection_AddQueryServices
{
	[Fact]
    public void With_DefaultQueryExecutor()
    {
        var sut = new ServiceCollection();
        sut.AddQueryServices();

        var serviceProvider = sut.BuildServiceProvider();

        var queryCriteriaHandler = serviceProvider.GetService<IQueryCriteriaHandler>();
        var expressionBuilder = serviceProvider.GetService<IExpressionBuilder>();
        var queryCriteriaFormatter = serviceProvider.GetService<IQueryCriteriaFormatter>();
        var queryCriteriaParser = serviceProvider.GetService<IQueryCriteriaParser>();
        var queryExecutor = serviceProvider.GetService<IQueryExecutor>();

        Assert.IsType<QueryCriteriaHandler>(queryCriteriaHandler);
        Assert.IsType<ExpressionBuilder>(expressionBuilder);
        Assert.IsType<ODataFormatter>(queryCriteriaFormatter);
        Assert.IsType<ODataParser>(queryCriteriaParser);
        Assert.IsType<DefaultQueryExecutor>(queryExecutor);
    }

    [Fact]
    public void With_CustomQueryExecutor()
    {
        var sut = new ServiceCollection();
        sut.AddQueryServices<CustomQueryExecutor>();

        var serviceProvider = sut.BuildServiceProvider();

        var queryCriteriaHandler = serviceProvider.GetService<IQueryCriteriaHandler>();
        var expressionBuilder = serviceProvider.GetService<IExpressionBuilder>();
        var queryCriteriaFormatter = serviceProvider.GetService<IQueryCriteriaFormatter>();
        var queryCriteriaParser = serviceProvider.GetService<IQueryCriteriaParser>();
        var queryExecutor = serviceProvider.GetService<IQueryExecutor>();

        Assert.IsType<QueryCriteriaHandler>(queryCriteriaHandler);
        Assert.IsType<ExpressionBuilder>(expressionBuilder);
        Assert.IsType<ODataFormatter>(queryCriteriaFormatter);
        Assert.IsType<ODataParser>(queryCriteriaParser);
        Assert.IsType<CustomQueryExecutor>(queryExecutor);
    }

    class CustomQueryExecutor : IQueryExecutor
    {
        public Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}