using System.Linq.Expressions;
using System.Reflection;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;


internal class ExpressionBuilder : IExpressionBuilder
{
    public Expression<Func<T, bool>> CreateFilterExpression<T>(FilterCriteria.FilterExpression filterExpression)
    {
        ArgumentNullException.ThrowIfNull(filterExpression);

        var parameter = CreateParameterExpression<T>();

        return filterExpression switch
        {
            FilterCriteria.LogicalExpression logicalExpression => CreateLogicalExpression<T>(logicalExpression, parameter),
            FilterCriteria.ComparisonExpression comparisonExpression => CreateComparisonExpression<T>(comparisonExpression, parameter),

            _ => throw new NotImplementedException($"Filter expression type {filterExpression.GetType().Name} is not implemented.")
        };
    }

    public ParameterExpression CreateParameterExpression<T>()
    {
        return Expression.Parameter(typeof(T));
    }

    public Expression<Func<T, object>> CreatePropertyOrFieldExpressionFromPath<T>(string propertyPath, ParameterExpression? parameter = null)
    {
        parameter ??= CreateParameterExpression<T>();

        var propertyPathExpression = CreatePropertyOrFieldExpressionFromPath(propertyPath, parameter);
        var converted = Expression.Convert(propertyPathExpression, typeof(object));
        var expression = Expression.Lambda<Func<T, object>>(converted, parameter);

        return expression;
    }

    private Expression<Func<T, bool>> CreateLogicalExpression<T>(
        FilterCriteria.LogicalExpression logicalExpression,
        ParameterExpression parameter
        )
    {
        if (logicalExpression.First == null || logicalExpression.Second == null)
        {
            throw new InvalidOperationException("At least two expressions must be provided for a logical expression.");
        }

        Expression? combinedBody = null;
        foreach (var expression in logicalExpression.AllExpressions)
        {
            var expressionLambda = CreateFilterExpression<T>(expression);
            var expressionVisitor = new ReplaceParameterVisitor(expressionLambda.Parameters[0], parameter);
            var expressionBody = expressionVisitor.Visit(expressionLambda.Body);

            combinedBody = combinedBody == null
                ? expressionBody
                : logicalExpression.Operator switch
                {
                    FilterCriteria.LogicalOperator.And => Expression.AndAlso(combinedBody, expressionBody),
                    FilterCriteria.LogicalOperator.Or => Expression.OrElse(combinedBody, expressionBody),
                    _ => throw new NotImplementedException($"Logical operator {logicalExpression.Operator} is not implemented.")
                };
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinedBody!, parameter);

        return lambda;
    }

    private Expression<Func<T, bool>> CreateComparisonExpression<T>(
        FilterCriteria.ComparisonExpression comparisonExpression,
        ParameterExpression parameter
        )
    {
        var property = CreatePropertyOrFieldExpressionFromPath(comparisonExpression.PropertyPath, parameter);
        var constant = CreateConstantExpression(comparisonExpression, property);

        ValidateOperatorOrThrow(comparisonExpression, property);

        var operatorExpression = CreateOperatorExpression(comparisonExpression, property, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(operatorExpression, parameter);

        return lambda;
    }

    private static ConstantExpression CreateConstantExpression(FilterCriteria.ComparisonExpression comparisonExpression, Expression property)
    {
        var value = GetValueFromExpression(comparisonExpression, property);

        var constantType = CreateConstantType(comparisonExpression, property);

        return Expression.Constant(value, constantType);
    }

    private static Type CreateConstantType(FilterCriteria.ComparisonExpression comparisonExpression, Expression property)
    {
        return comparisonExpression.Operator == FilterCriteria.ComparisonOperator.In
            ? typeof(IEnumerable<>).MakeGenericType(property.Type)
            : property.Type;
    }

    private static void ValidateOperatorOrThrow(FilterCriteria.ComparisonExpression comparisonExpression, Expression property)
    {
        bool isStringOperator = HasStringOperator(comparisonExpression);

        if (isStringOperator && property.Type != typeof(string))
        {
            throw new InvalidOperationException($"Operator {comparisonExpression.Operator} can only be applied to string properties.");
        }
    }

    private static bool HasStringOperator(FilterCriteria.ComparisonExpression comparisonExpression)
    {
        var stringExpressions = new[]
        {
            FilterCriteria.ComparisonOperator.Contains,
            FilterCriteria.ComparisonOperator.StartsWith,
            FilterCriteria.ComparisonOperator.EndsWith
        };

        var isStringOperator = stringExpressions.Contains(comparisonExpression.Operator);
        return isStringOperator;
    }

    private Expression CreateOperatorExpression(FilterCriteria.ComparisonExpression comparisonExpression, Expression property, ConstantExpression constant)
    {
        return comparisonExpression.Operator switch
        {
            FilterCriteria.ComparisonOperator.Equals => Expression.Equal(property, constant),
            FilterCriteria.ComparisonOperator.NotEquals => Expression.NotEqual(property, constant),
            FilterCriteria.ComparisonOperator.GreaterThan => Expression.GreaterThan(property, constant),
            FilterCriteria.ComparisonOperator.LessThan => Expression.LessThan(property, constant),
            FilterCriteria.ComparisonOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
            FilterCriteria.ComparisonOperator.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
            FilterCriteria.ComparisonOperator.Contains => Expression.Call(property, GetStringMethodInfo(nameof(string.Contains)), constant),
            FilterCriteria.ComparisonOperator.StartsWith => Expression.Call(property, GetStringMethodInfo(nameof(string.StartsWith)), constant),
            FilterCriteria.ComparisonOperator.EndsWith => Expression.Call(property, GetStringMethodInfo(nameof(string.EndsWith)), constant),
            FilterCriteria.ComparisonOperator.In => Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), [property.Type], constant, property),

            _ => throw new NotImplementedException($"Operator {comparisonExpression.Operator} is not implemented.")
        };
    }


    private static object? GetValueFromExpression(FilterCriteria.ComparisonExpression comparisonExpression, Expression property)
    {
        var value = comparisonExpression.Value;

        if (value == null)
            return null;

        if (comparisonExpression.Operator == FilterCriteria.ComparisonOperator.In)
        {
            return GetInValues(property, value);
        }

        var valueType = value.GetType();
        var propertyType = property.Type;

        if(valueType == typeof(DateTime) && propertyType == typeof(DateTimeOffset))
        {
            return new DateTimeOffset((DateTime)value);
        }

        if(valueType == typeof(DateTimeOffset) && propertyType == typeof(DateTime))
        {
            return ((DateTimeOffset)value).DateTime;
        }

        var needsConversion = valueType != propertyType;
        value = needsConversion ? Convert.ChangeType(value, propertyType) : value;

        return value;
    }

    private static object GetInValues(Expression property, object value)
    {
        var valueType = value.GetType();
        var elementType = valueType.GetElementType() ?? valueType.GetGenericArguments().FirstOrDefault();
        if (elementType == null)
        {
            throw new InvalidOperationException("Value for 'In' operator must be a collection.");
        }

        var propertyType = property.Type;
        if(elementType == propertyType)
            return value;

        var enumerableValue = value as System.Collections.IEnumerable;
        if (enumerableValue == null)
        {
            throw new InvalidOperationException("Value for 'In' operator must be a collection.");
        }

        var listType = typeof(List<>).MakeGenericType(propertyType);
        var listInstance = (System.Collections.IList)Activator.CreateInstance(listType)!;

        foreach (var elementValue in enumerableValue)
        {
            var convertedItem = Convert.ChangeType(elementValue, propertyType);
            listInstance.Add(convertedItem);
        }

        return listInstance;
    }

    private Expression CreatePropertyOrFieldExpressionFromPath(string propertyPath, ParameterExpression parameter)
    {
        var currentExpression = parameter as Expression;
        var properties = propertyPath.Split('.');

        foreach (var propertyName in properties)
        {
            currentExpression = Expression.PropertyOrField(currentExpression, propertyName);
        }

        return currentExpression;
    }

    private MethodInfo GetStringMethodInfo(string methodName)
    {
        var arguments = new[] { typeof(string) };
        return typeof(string).GetMethod(methodName, arguments)!;
    }

    internal class ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam) : ExpressionVisitor
    {

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == oldParam ? newParam : base.VisitParameter(node);
        }
    }
}
