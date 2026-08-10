using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

public class QueryCriteriaDefaultToolkit : IQueryCriteriaToolkit
{
    private readonly IQueryCriteriaFormatter formatter = new ODataFormatter();
    private readonly IQueryCriteriaParser parser = new ODataParser();
    private readonly IQueryCriteriaHandler queryCriteriaHandler;

    private readonly IExpressionBuilder expressionBuilder = new ExpressionBuilder();

    public QueryCriteriaDefaultToolkit(IQueryExecutor queryExecutor)
    {
        Executor = queryExecutor;
        queryCriteriaHandler = new QueryCriteriaHandler(expressionBuilder, queryExecutor);
    }

    public IQueryCriteriaFormatter Formatter => formatter;

    public IQueryCriteriaParser Parser => parser;

    public IQueryCriteriaHandler Handler => queryCriteriaHandler;

    public IQueryExecutor Executor { get; private init;}
    
    public IExpressionBuilder ExpressionBuilder => expressionBuilder;
}
