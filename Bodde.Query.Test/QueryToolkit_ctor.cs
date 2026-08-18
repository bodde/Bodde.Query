using Bodde.Query.Abstractions.Services;
using Bodde.Query.Core;
using Moq;

namespace Bodde.Query.Test;

public class QueryToolkit_ctor
{
    [Fact]
    public async Task Tools_AreAvailable()
    {
        var sut = QueryToolkit.Default();

        var formatter = sut.Formatter;
        var parser = sut.Parser;
        var handler = sut.Handler;
        var executor = sut.Executor;
        var expressionBuilder = sut.ExpressionBuilder;

        Assert.NotNull(formatter);
        Assert.IsType<ODataFormatter>(formatter);

        Assert.NotNull(parser);
        Assert.IsType<ODataParser>(parser);

        Assert.NotNull(handler);
        Assert.IsType<QueryCriteriaHandler>(handler);

        Assert.NotNull(executor);
        Assert.IsType<DefaultQueryExecutor>(executor);

        Assert.NotNull(expressionBuilder);
        Assert.IsType<ExpressionBuilder>(expressionBuilder);
    }

        [Fact]
    public async Task Custom_Tools_AreAvailable()
    {
        var customFormatter = new Mock<IQueryCriteriaFormatter>();
        var customParser = new Mock<IQueryCriteriaParser>();
        var customExpressionBuilder = new Mock<IExpressionBuilder>();
        var customHandler = new Mock<IQueryCriteriaHandler>();
        var customExecutor = new Mock<IQueryExecutor>();


        var sut = new QueryToolkit(
            customFormatter.Object, 
            customParser.Object,
            customExpressionBuilder.Object,
            customHandler.Object,
            customExecutor.Object
            );

        var actualFormatter = sut.Formatter;
        var actualParser = sut.Parser;
        var actualExpressionBuilder = sut.ExpressionBuilder;
        var actualHandler = sut.Handler;
        var actualExecutor = sut.Executor;

        Assert.Same(customFormatter.Object, actualFormatter);
        Assert.Same(customParser.Object, actualParser);
        Assert.Same(customExpressionBuilder.Object, actualExpressionBuilder);
        Assert.Same(customHandler.Object, actualHandler);
        Assert.Same(customExecutor.Object, actualExecutor);
    }

}