using System;
using System.Linq.Expressions;
using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Abstractions.Services;

public interface IExpressionBuilder
{
    Expression<Func<T, bool>> CreateFilterExpression<T>(FilterCriteria.FilterExpression filterExpression);
    ParameterExpression CreateParameterExpression<T>();
    Expression<Func<T, object>> CreatePropertyOrFieldExpressionFromPath<T>(string propertyPath, ParameterExpression? parameter = null);
}
