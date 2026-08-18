using Bodde.Query.Abstractions.Services;
using Moq;

namespace Bodde.Query.Test.Mocked;

internal class QueryToolkitMock : Mock<IQueryToolkit>
{
    public QueryToolkitMock()
    {
        Formatter = new QueryCriteriaFormatterMock();
        Parser = new QueryCriteriaParserMock();
        ExpressionBuilder = new ExpressionBuilderMock();
        Handler = new QueryCriteriaHandlerMock();
        Executor = new QueryExecutorMock();

        SetupGet(_ => _.Formatter).Returns(Formatter.Object);
        SetupGet(_ => _.Parser).Returns(Parser.Object);
        SetupGet(_ => _.ExpressionBuilder).Returns(ExpressionBuilder.Object);
        SetupGet(_ => _.Handler).Returns(Handler.Object);
        SetupGet(_ => _.Executor).Returns(Executor.Object);
    }

    public QueryCriteriaFormatterMock Formatter { get; }
    public QueryCriteriaParserMock Parser { get; }
    public ExpressionBuilderMock ExpressionBuilder { get; }
    public QueryCriteriaHandlerMock Handler { get; }
    public QueryExecutorMock Executor { get; }
}
