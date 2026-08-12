using Bodde.Query.Abstractions.Services;
using Bodde.Query.Core;

public class QueryToolkit(
    IQueryCriteriaParser? queryCriteriaParser = null,
    IQueryCriteriaFormatter? queryCriteriaFormatter = null,
    IExpressionBuilder? expressionBuilder = null,
    IQueryCriteriaHandler? queryCriteriaHandler = null,
    IQueryExecutor? queryExecutor = null
    ) : IQueryToolkit
{
    public IQueryCriteriaFormatter Formatter => queryCriteriaFormatter ?? new ODataFormatter();

    public IQueryCriteriaParser Parser => queryCriteriaParser ?? new ODataParser();

    public IQueryCriteriaHandler Handler => queryCriteriaHandler ?? new QueryCriteriaHandler(ExpressionBuilder, Executor);

    public IQueryExecutor Executor => queryExecutor ?? new DefaultQueryExecutor();

    public IExpressionBuilder ExpressionBuilder => expressionBuilder ?? new ExpressionBuilder();
}