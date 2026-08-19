using System.Linq.Expressions;
using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

/// <summary>
/// The service that builds LINQ expressions from filter criteria.
/// </summary>
public interface IExpressionBuilder
{
    /// <summary>
    /// Creates a predicate expression from a filter expression.
    /// </summary>
    /// <typeparam name="T">The type of the objects evaluated by the predicate.</typeparam>
    /// <param name="filterExpression">The filter expression to convert.</param>
    /// <returns>A predicate expression for objects of type <typeparamref name="T"/>.</returns>
    Expression<Func<T, bool>> CreateFilterExpression<T>(FilterCriteria.FilterExpression filterExpression);

    /// <summary>
    /// Creates an expression that accesses a property or field through a dotted path.
    /// </summary>
    /// <typeparam name="T">The type containing the property or field.</typeparam>
    /// <param name="propertyPath">The dotted property or field path.</param>
    /// <returns>An expression that accesses the specified property or field.</returns>
    Expression<Func<T, object>> CreatePropertyOrFieldExpressionFromPath<T>(string propertyPath);
}
