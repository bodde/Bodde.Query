using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

public class DefaultQueryToolkit(
    IQueryCriteriaParser? parser = null,
    IQueryCriteriaFormatter? formatter = null,
    IExpressionBuilder? expressionBuilder = null,
    IQueryCriteriaHandler? handler = null,
    IQueryExecutor? executor = null
    ) : IQueryToolkit
{
    public IQueryCriteriaFormatter Formatter => formatter ?? new ODataFormatter();

    public IQueryCriteriaParser Parser => parser ?? new ODataParser();

    public IQueryCriteriaHandler Handler => handler ?? new QueryCriteriaHandler(ExpressionBuilder, this);

    public IQueryExecutor Executor => executor ?? new DefaultQueryExecutor();

    public IExpressionBuilder ExpressionBuilder => expressionBuilder ?? new ExpressionBuilder();
}