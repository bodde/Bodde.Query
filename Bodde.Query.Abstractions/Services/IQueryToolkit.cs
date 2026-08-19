namespace Bodde.Query.Abstractions.Services;

/// <summary>
/// Provides access to the services used to parse, format, build, apply, and execute query criteria.
/// </summary>
public interface IQueryToolkit
{
    /// <summary>
    /// Gets the service that formats query criteria as strings.
    /// </summary>
    IQueryCriteriaFormatter Formatter { get; }

    /// <summary>
    /// Gets the service that parses query criteria from strings or query parameters.
    /// </summary>
    IQueryCriteriaParser Parser { get; }
    
    /// <summary>
    /// Gets the service that builds LINQ expressions from filter criteria.
    /// </summary>
    IExpressionBuilder ExpressionBuilder { get; }

    /// <summary>
    /// Gets the service that applies query criteria to an <see cref="IQueryable{T}"/>.
    /// </summary>
    IQueryCriteriaHandler Handler { get; }

    /// <summary>
    /// Gets the service that executes queries and creates query results.
    /// </summary>
    IQueryExecutor Executor { get; }


}
