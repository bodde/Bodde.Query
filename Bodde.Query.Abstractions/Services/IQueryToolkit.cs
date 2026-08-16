namespace Bodde.Query.Abstractions.Services;

public interface IQueryToolkit
{
    IQueryCriteriaFormatter Formatter { get; }

    IQueryCriteriaParser Parser { get; }    
    
    IExpressionBuilder ExpressionBuilder { get; }

    IQueryCriteriaHandler Handler { get; }

    IQueryExecutor Executor { get; }


}
