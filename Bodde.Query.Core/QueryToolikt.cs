using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

public class QueryToolkit(
        IQueryCriteriaFormatter formatter,
        IQueryCriteriaParser parser,
        IExpressionBuilder expressionBuilder,
        IQueryCriteriaHandler handler,
        IQueryExecutor executor
    ) : IQueryToolkit
{
    public static QueryToolkit Default(
        IQueryCriteriaFormatter? formatter = null,
        IQueryCriteriaParser? parser = null,
        IExpressionBuilder? expressionBuilder = null,
        IQueryCriteriaHandler? handler = null,
        IQueryExecutor? executor = null
        )
    {
        formatter ??= new ODataFormatter();
        parser ??= new ODataParser();
        expressionBuilder ??= new ExpressionBuilder();
        handler ??= new QueryCriteriaHandler(expressionBuilder);
        executor ??= new DefaultQueryExecutor();

        return new QueryToolkit(formatter, parser, expressionBuilder, handler, executor);
    }

    public IQueryCriteriaFormatter Formatter { get; } = formatter;
    public IQueryCriteriaParser Parser { get; } = parser;
    public IExpressionBuilder ExpressionBuilder { get; } = expressionBuilder;
    public IQueryCriteriaHandler Handler { get; } = handler;
    public IQueryExecutor Executor { get; } = executor;

}