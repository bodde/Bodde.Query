namespace Bodde.Query.Abstractions.Services;

public interface IQueryCriteriaToolkit
{
    IQueryCriteriaFormatter Formatter { get; }

    IQueryCriteriaParser Parser { get; }

    IQueryCriteriaHandler Handler { get; }

    IQueryExecutor Executor { get; }

    IExpressionBuilder ExpressionBuilder { get; }
}
