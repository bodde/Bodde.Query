using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

/// <summary>
/// Provides the query services used to parse, format, build, apply, and execute query criteria.
/// </summary>
/// <param name="formatter">The service used to format query criteria.</param>
/// <param name="parser">The service used to parse query criteria.</param>
/// <param name="expressionBuilder">The service used to build LINQ expressions.</param>
/// <param name="handler">The service used to apply query criteria.</param>
/// <param name="executor">The service used to execute queries.</param>
public class QueryToolkit(
        IQueryCriteriaFormatter formatter,
        IQueryCriteriaParser parser,
        IExpressionBuilder expressionBuilder,
        IQueryCriteriaHandler handler,
        IQueryExecutor executor
    ) : IQueryToolkit
{
    /// <summary>
    /// Creates a toolkit using the specified services or the default Core implementations.
    /// </summary>
    /// <param name="formatter">The formatter to use, or <see langword="null"/> for the default formatter.</param>
    /// <param name="parser">The parser to use, or <see langword="null"/> for the default parser.</param>
    /// <param name="expressionBuilder">The expression builder to use, or <see langword="null"/> for the default builder.</param>
    /// <param name="handler">The criteria handler to use, or <see langword="null"/> for the default handler.</param>
    /// <param name="executor">The query executor to use, or <see langword="null"/> for the default executor.</param>
    /// <returns>A toolkit configured with the specified or default services.</returns>
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
        executor ??= new LinqQueryExecutor();

        return new QueryToolkit(formatter, parser, expressionBuilder, handler, executor);
    }

    /// <summary>Gets the service used to format query criteria.</summary>
    public IQueryCriteriaFormatter Formatter { get; } = formatter;
    /// <summary>Gets the service used to parse query criteria.</summary>
    public IQueryCriteriaParser Parser { get; } = parser;
    /// <summary>Gets the service used to build LINQ expressions.</summary>
    public IExpressionBuilder ExpressionBuilder { get; } = expressionBuilder;
    /// <summary>Gets the service used to apply query criteria.</summary>
    public IQueryCriteriaHandler Handler { get; } = handler;
    /// <summary>Gets the service used to execute queries.</summary>
    public IQueryExecutor Executor { get; } = executor;

}