using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

public class DefaultQueryToolkit : IQueryToolkit
{
    public DefaultQueryToolkit(
        IQueryCriteriaFormatter? formatter = null,
        IQueryCriteriaParser? parser = null,
        IExpressionBuilder? expressionBuilder = null,
        IQueryCriteriaHandler? handler = null,
        IQueryExecutor? executor = null
    )
    {
        Formatter = formatter ?? new ODataFormatter();
        Parser = parser ?? new ODataParser();
        ExpressionBuilder = expressionBuilder ?? new ExpressionBuilder();
        Handler = handler ?? new QueryCriteriaHandler(this);
        Executor = executor ?? new DefaultQueryExecutor();
    }

    public IQueryCriteriaFormatter Formatter { get; }
    public IQueryCriteriaParser Parser { get; }    
    public IExpressionBuilder ExpressionBuilder { get; }
    public IQueryCriteriaHandler Handler { get; }
    public IQueryExecutor Executor { get; }

}