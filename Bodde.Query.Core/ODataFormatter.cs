using Bodde.Common.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;
using static Bodde.Query.Abstractions.Models.FilterCriteria;

namespace Bodde.Query.Core;

internal class ODataFormatter : IQueryCriteriaFormatter
{
    public string Format(QueryCriteria criteria)
    {
        var parts = new List<string>();
  
        if (criteria.Paging != null)
            parts.Add(FormatPaging(criteria.Paging));

        if (criteria.Filter != null)
            parts.Add(FormatFilter(criteria.Filter)); 

        if (criteria.OrderBy != null && criteria.OrderBy.Items.Length > 0)
            parts.Add(FormatOrderBy(criteria.OrderBy)); 

        return string.Join("&", parts);
    }


    public string FormatPaging(PagingCriteria paging)
    {
        if (paging == null)
            throw new ArgumentNullException(nameof(paging));

        var parts = new List<string>();

        if (paging.Skip.HasValue)
            parts.Add($"$skip={paging.Skip.Value}");

        if (paging.Top.HasValue) 
            parts.Add($"$top={paging.Top.Value}");

        if (paging.TotalCount.HasValue) 
            parts.Add($"$count={paging.TotalCount.Value.ToString().ToLowerInvariant()}");

        return string.Join("&", parts);
    }


    public string FormatFilter(FilterCriteria filter)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

        return $"$filter={FormatFilterExpression(filter.Expression)}";
    }

    public string FormatOrderBy(OrderByCriteria orderBy)
    {
        if (orderBy == null)
            throw new ArgumentNullException(nameof(orderBy));

        if (orderBy.Items.Length == 0)
            return string.Empty;

        var orderingStrings = orderBy.Items
            .Select(_ => $"{_.PropertyPath} {ConvertDirectionToString(_.Direction)}")
            .ToArray();

        return $"$orderby={string.Join(",", orderingStrings)}";
    }

    private string FormatFilterExpression(FilterExpression expression)
    {
        return expression switch
        {
            ComparisonExpression comparison => FormatComparisonExpression(comparison),
            NotExpression notExpression => FormatNotExpression(notExpression),
            LogicalExpression logicalExpression => FormatLogicalExpression(logicalExpression),
            _ => throw new NotSupportedException($"Unsupported filter expression type: {expression.GetType().Name}")
        };
    }

    private string FormatNotExpression(NotExpression notExpression)
    {
        return $"not ({FormatFilterExpression(notExpression.Expression)})";
    }

    private static string FormatComparisonExpression(ComparisonExpression comparison)
    {
        return $"{comparison.PropertyPath} {ConvertComparisonOperatorToString(comparison.Operator)} {FormatValue(comparison.Value)}";
    }

    private string FormatLogicalExpression(LogicalExpression logicalExpression)
    {                
        var expressions = logicalExpression.AllExpressions.Select(_ => FormatFilterExpression(_)).ToArray();
        string operatorString = ConvertLogicalOperatorToString(logicalExpression);

        return string.Join($" {operatorString} ", expressions);
    }

    private static string ConvertComparisonOperatorToString(ComparisonOperator comparisonOperator)
    {
        return comparisonOperator switch
        {
            ComparisonOperator.Equals => "eq",
            ComparisonOperator.NotEquals => "ne",
            ComparisonOperator.GreaterThan => "gt",
            ComparisonOperator.LessThan => "lt",
            ComparisonOperator.GreaterThanOrEqual => "ge",
            ComparisonOperator.LessThanOrEqual => "le",
            ComparisonOperator.Contains => "contains",
            ComparisonOperator.StartsWith => "startswith",
            ComparisonOperator.EndsWith => "endswith",
            ComparisonOperator.In => "in",
            _ => throw new NotSupportedException($"Unsupported comparison operator: {comparisonOperator}")
        };
    }

    private static string ConvertLogicalOperatorToString(LogicalExpression logicalExpression)
    {
        return logicalExpression.Operator switch
        {
            LogicalOperator.And => "and",
            LogicalOperator.Or => "or",
            _ => throw new NotSupportedException($"Unsupported logical operator: {logicalExpression.Operator}")
        };
    }

    private string ConvertDirectionToString(OrderByCriteria.SortDirection direction)
    {
        return direction switch
        {
            OrderByCriteria.SortDirection.Ascending => "asc",
            OrderByCriteria.SortDirection.Descending => "desc",
            _ => throw new NotSupportedException($"Unsupported sort direction: {direction}")
        };
    }

    private static string FormatValue(object? value)
    {
        if (value is string strValue)
        {
            return $"'{strValue}'";
        }
        else if (value is DateTime dateTimeValue)
        {
            return dateTimeValue.ToString("o"); // ISO 8601 format
        }
        else if (value is bool boolValue)
        {
            return boolValue.ToString().ToLower();
        }
        else if (value is null)
        {
            return "null";
        }
        else if (value is System.Collections.IEnumerable enumerableValue && value is not string)
        {
            var objectEnumerableValue = enumerableValue.Cast<object>(); 

            var valueListCsv = objectEnumerableValue.ToCsv(_ => FormatValue(_), separator: ", ");
            return $"({valueListCsv})";
        }
        else
        {
            return value.ToString() ?? string.Empty;
        }   
    }
}