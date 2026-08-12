using System.Globalization;
using System.Text.RegularExpressions;
using Bodde.Common.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

internal partial class ODataParser : IQueryCriteriaParser
{
    private static readonly Dictionary<string, FilterCriteria.ComparisonOperator> _comparisonOperators = new()
    {
        {"eq", FilterCriteria.ComparisonOperator.Equals},
        {"ne", FilterCriteria.ComparisonOperator.NotEquals},
        {"gt", FilterCriteria.ComparisonOperator.GreaterThan},
        {"ge", FilterCriteria.ComparisonOperator.GreaterThanOrEqual},
        {"lt", FilterCriteria.ComparisonOperator.LessThan},
        {"le", FilterCriteria.ComparisonOperator.LessThanOrEqual},
        {"contains", FilterCriteria.ComparisonOperator.Contains},
        {"startswith", FilterCriteria.ComparisonOperator.StartsWith},
        {"endswith", FilterCriteria.ComparisonOperator.EndsWith},
        {"in", FilterCriteria.ComparisonOperator.In}
    };
    
    public PagingCriteria? ParsePaging(string pagingString)
    {
        if (string.IsNullOrWhiteSpace(pagingString))
            return null;

        var skipValue = SkipRegex().Match(pagingString).Groups[1].Value;
        var topValue = TopRegex().Match(pagingString).Groups[1].Value;
        var countValue = CountRegex().Match(pagingString).Groups[1].Value;

        if(string.IsNullOrEmpty(skipValue) && string.IsNullOrEmpty(topValue) && string.IsNullOrEmpty(countValue))
            return null;

        return new PagingCriteria(
            Skip: string.IsNullOrEmpty(skipValue) ? null : int.Parse(skipValue),
            Top: string.IsNullOrEmpty(topValue) ? null : int.Parse(topValue),
            TotalCount: string.IsNullOrEmpty(countValue) ? null : bool.Parse(countValue)
        );
    }

    public FilterCriteria? ParseFilter(string filterString)
    {
        if(filterString == null) 
            return null;

        filterString = filterString
            .Replace("$filter=", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (string.IsNullOrEmpty(filterString))
            return null;

        var expressionsBag = new Dictionary<string, FilterCriteria.FilterExpression>();

        filterString = ProcessComparisonExpressions(filterString, expressionsBag);
        filterString = ProcessNotAndLogicalExpressions(filterString, expressionsBag);

        var topLevelExpression = CreateTopLevelExpression(filterString, expressionsBag);

        return new FilterCriteria(topLevelExpression);
    }

    public OrderByCriteria? ParseOrderBy(string orderByString)
    {
        if(orderByString == null) 
            return null;

        orderByString = orderByString
            .Replace("$orderby=", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (string.IsNullOrEmpty(orderByString))
            return null;

        var orderByItems = orderByString
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(_ => ParseOrderByItem(_))
            .ToArray();

        return new OrderByCriteria(orderByItems);     
    }

    public QueryCriteria? Parse(string criteriaString)
    {        
        if(string.IsNullOrWhiteSpace(criteriaString)) 
            return null;

        var paging = ParsePaging(criteriaString);

        var parts = criteriaString.Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var filterCriteriaString = parts.FirstOrDefault(_ => _.StartsWith("$filter=", StringComparison.OrdinalIgnoreCase));
        var filter = filterCriteriaString != null ? ParseFilter(filterCriteriaString) : null;

        var orderByCriteriaString = parts.FirstOrDefault(_ => _.StartsWith("$orderby=", StringComparison.OrdinalIgnoreCase));
        var orderBy = orderByCriteriaString != null ? ParseOrderBy(orderByCriteriaString) : null;

        return new QueryCriteria(Filter: filter, Paging: paging, OrderBy: orderBy);
    }

    public QueryCriteria? Parse(QueryCriteriaParameters? queryCriteriaParameters)
    {
        if (queryCriteriaParameters == null || !queryCriteriaParameters.AreSet)
            return null;

        var paging = new PagingCriteria(
            Skip: queryCriteriaParameters.Skip,
            Top: queryCriteriaParameters.Top,
            TotalCount: queryCriteriaParameters.Count
        );

        var filter = !string.IsNullOrWhiteSpace(queryCriteriaParameters.Filter)
            ? ParseFilter(queryCriteriaParameters.Filter!)
            : null;

        var orderBy = !string.IsNullOrWhiteSpace(queryCriteriaParameters.OrderBy)
            ? ParseOrderBy(queryCriteriaParameters.OrderBy!)
            : null;

        return new QueryCriteria(
            Paging: paging,
            Filter: filter,
            OrderBy: orderBy
        );
    }


    private string ProcessComparisonExpressions(string filterString, Dictionary<string, FilterCriteria.FilterExpression> expressionsBag)
    {
        var comparisonStatements = GetComparisonStatements(filterString);
        if (comparisonStatements.Length == 0)
        {
            throw new NotImplementedException("No comparison statements found in filter string.");
        }

        foreach (var comparisonStatement in comparisonStatements)
        {
            string expressionKey = NextExpressionKey(expressionsBag);

            expressionsBag[expressionKey] = CreateComparisonExpression(comparisonStatement);

            filterString = filterString.Replace(comparisonStatement, expressionKey);
        }

        return filterString;
    }



    private string ProcessNotAndLogicalExpressions(string filterString, Dictionary<string, FilterCriteria.FilterExpression> expressionsBag)
    {
        while (true)
        {
            filterString = ProcessNotExpressions(filterString, expressionsBag);

            var innerExpressions = GetInnerLogicalExpressions(filterString);
            if (innerExpressions.Length == 0)
                break;

            filterString = ProcessInnerLogicalExpressions(filterString, innerExpressions, expressionsBag);
        }

        return filterString;
    }

    private static string ProcessNotExpressions(string filterString, Dictionary<string, FilterCriteria.FilterExpression> expressionsBag)
    {
        var notMatches = NotExpressionsRegex().Matches(filterString);
        foreach (Match notMatch in notMatches)
        {
            var notStatement = notMatch.Value;
            var innerExpressionKey = notMatch.Groups["key1"].Success
                ? notMatch.Groups["key1"].Value
                : notMatch.Groups["key2"].Success
                ? notMatch.Groups["key2"].Value
                : throw new InvalidOperationException("No valid group found for not expression.");

            var innerExpression = expressionsBag[innerExpressionKey];

            var notExpression = new FilterCriteria.NotExpression(innerExpression);

            var notExpressionKey = NextExpressionKey(expressionsBag);
            expressionsBag[notExpressionKey] = notExpression;

            filterString = filterString.Replace(notStatement, notExpressionKey);
        }

        return filterString;
    }


    private static string[] GetInnerLogicalExpressions(string filterString)
    {
        return SurroundedByParenthesesRegex().Matches(filterString)
                .Select(m => m.Value)
                .ToArray();
    }

    private string ProcessInnerLogicalExpressions(
        string filterString,
        string[] innerExpressions,
        Dictionary<string, FilterCriteria.FilterExpression> expressionsBag
        )
    {
        foreach (var innerExpression in innerExpressions)
        {
            var expression = innerExpression[1..^1]; // remove surrounding parentheses

            var logicalExpression = CreateLogicalExpression(expression, expressionsBag);

            var logicalExpressionKey = NextExpressionKey(expressionsBag);
            expressionsBag[logicalExpressionKey] = logicalExpression;

            filterString = filterString.Replace(innerExpression, logicalExpressionKey);
        }
        return filterString;
    }

    private FilterCriteria.FilterExpression CreateTopLevelExpression(string filterString, Dictionary<string, FilterCriteria.FilterExpression> expressionsBag)
    {
        if (LogicalOperatorsRegex().IsMatch(filterString))
        {
            return CreateLogicalExpression(filterString, expressionsBag);
        }

        if (expressionsBag.ContainsKey(filterString))
            return expressionsBag[filterString];

        throw new InvalidOperationException("Unable to create top-level expression from filter string.");
    }

    private static string NextExpressionKey(Dictionary<string, FilterCriteria.FilterExpression> expressionsBag)
    {
        return $"|{expressionsBag.Count}|";
    }

    private FilterCriteria.LogicalExpression CreateLogicalExpression(
        string expressionString,
        Dictionary<string, FilterCriteria.FilterExpression> expressionsBag
        )
    {
        var logicalOperator = GetLogicalOperator(expressionString);
        var expressionKeys = GetExpressionKeys(expressionString);

        if (expressionKeys.Length < 2)
        {
            throw new InvalidOperationException("At least two expressions are required to create a logical expression.");
        }

        var expressions = expressionKeys
            .Select(key => expressionsBag[key])
            .ToArray();

        var logicalExpression = new FilterCriteria.LogicalExpression(
            Operator: logicalOperator,
            First: expressions[0],
            Second: expressions[1],
            Others: expressions.Length > 2 ? expressions[2..] : []
            );

        return logicalExpression;
    }

    private string[] GetExpressionKeys(string expressionString)
    {

        var expressionKeys = ExpressionKeysRegex()
            .Matches(expressionString)
            .Select(m => m.Value)
            .ToArray();

        return expressionKeys;
    }

    private FilterCriteria.LogicalOperator GetLogicalOperator(string filterString)
    {
        var logicalOperators = LogicalOperatorsRegex()
            .Matches(filterString)
            .Select(m => m.Value.Trim().ToLower())
            .ToArray();

        if (logicalOperators.Distinct().Count() > 1)
        {
            throw new NotImplementedException("Only one logical operator per logical expression is supported.");
        }

        var logicalOperator = ConvertLogicalOperatorStringToEnum(logicalOperators[0]);
        return logicalOperator;
    }

    private FilterCriteria.LogicalOperator ConvertLogicalOperatorStringToEnum(string value)
    {
        return value switch
        {
            "and" => FilterCriteria.LogicalOperator.And,
            "or" => FilterCriteria.LogicalOperator.Or,
            _ => throw new NotImplementedException($"Logical operator '{value}' is not supported.")
        };
    }


    private FilterCriteria.ComparisonExpression CreateComparisonExpression(string comparisonStatement)
    {
        var parts = ExpressionRegex().Matches(comparisonStatement.Trim()).Select(m => m.Value).ToArray();

        if (parts.Length != 3)
        {
            throw new NotImplementedException("Only simple comparison expressions are supported in this example.");
        }

        var propertyPath = parts[0];
        var operatorString = parts[1];
        var valueString = parts[2];

        var comparisonOperator = ConvertODataOperatorToComparisonOperator(operatorString);
        var value = ConvertValueString(valueString, comparisonOperator);

        var comparisonExpression = new FilterCriteria.ComparisonExpression(
            PropertyPath: propertyPath,
            Operator: comparisonOperator,
            Value: value
        );
        return comparisonExpression;
    }

    public string[] GetComparisonStatements(string filterString)
    {
        var comparisonStatements = ComparisonStatementsRegex()
            .Matches(filterString)
            .Select(m => m.Value).ToArray();

        if (comparisonStatements.Length == 0)
        {
            throw new FormatException("No comparison statements found in filter string.");
        }

        return comparisonStatements;
    }

    private object? ConvertValueString(string valueString, FilterCriteria.ComparisonOperator comparisonOperator)
    {
        if (comparisonOperator == FilterCriteria.ComparisonOperator.In)
        {
            // handle 'in' operator with multiple values in parentheses
            var inValuesRegex = new Regex(@"\((.*)\)");
            var match = inValuesRegex.Match(valueString);
            if (!match.Success)
            {
                throw new NotImplementedException("Invalid syntax for 'in' operator.");
            }

            var valuesPart = match.Groups[1].Value;
            var valueTypes = valuesPart.Split(',')
                .Select(v => v.Trim())
                .Select(v => { var (value, type) = ParseValue(v); return new { value, type }; })
                .ToArray();

            if (valueTypes.DistinctBy(vt => vt.type).Count() > 1)
            {
                throw new NotImplementedException("All values for 'in' operator must be of the same type.");
            }

            // convert to array of the appropriate type
            var elementType = valueTypes.First().type;
            var arrayType = elementType.MakeArrayType();
            var values = Array.CreateInstance(elementType, valueTypes.Length);
            for (int i = 0; i < valueTypes.Length; i++)
            {
                values.SetValue(valueTypes[i].value, i);
            }

            return values;
        }

        return ParseValue(valueString).Item1;
    }

    internal static (object?, Type) ParseValue(string valueString)
    {
        if (valueString.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return (null!, typeof(object));
        }

        var quotesRegEx = QuotesRegex();
        if (quotesRegEx.IsMatch(valueString))
        {
            return (quotesRegEx.Replace(valueString, "$1"), typeof(string));
        }

        if (int.TryParse(valueString, out var intValue))
        {
            return (intValue, typeof(int));
        }
        if (double.TryParse(valueString, CultureInfo.InvariantCulture, out var doubleValue))
        {
            return (doubleValue, typeof(double));
        }
        if (bool.TryParse(valueString, out var boolValue))
        {
            return (boolValue, typeof(bool));
        }
        if (DateTime.TryParse(
            valueString,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var dateTimeValue
            ))
        {
            return (dateTimeValue, typeof(DateTime));
        }

        throw new NotImplementedException($"Unable to parse value string '{valueString}'.");
    }

    private FilterCriteria.ComparisonOperator ConvertODataOperatorToComparisonOperator(string operatorString)
    {
        if (_comparisonOperators.TryGetValue(operatorString, out var comparisonOperator))
        {
            return comparisonOperator;
        }

        throw new InvalidOperationException($"OData operator '{operatorString}' is not supported. Supported operators are: {_comparisonOperators.Keys.ToCsv()}");
    }

    private static OrderByCriteria.OrderByItem ParseOrderByItem(string itemString)
    {
        var parts = itemString.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var propertyPath = parts[0];
        var direction = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? OrderByCriteria.SortDirection.Descending
            : OrderByCriteria.SortDirection.Ascending;
        return new OrderByCriteria.OrderByItem(
            PropertyPath: propertyPath,
            Direction: direction
        );
    }

    [GeneratedRegex(@"([\w\.]+\s+(?:eq|ne|gt|ge|lt|le|contains|startswith|endswith|in)\s+(?:null|true|false|'[^']*'|\(.+\)|[\d\-T\:\.Z]+))", RegexOptions.IgnoreCase)]
    private static partial Regex ComparisonStatementsRegex();

    [GeneratedRegex(@"\s+(and|or)\s+", RegexOptions.IgnoreCase)]
    private static partial Regex LogicalOperatorsRegex();

    [GeneratedRegex(@"(?:not\s+\((?'key1'\|\d+\|)\))|(?:not\s+(?'key2'\|\d+\|))", RegexOptions.IgnoreCase)]
    private static partial Regex NotExpressionsRegex();

    [GeneratedRegex(@"\(([^()]+)\)")]
    private static partial Regex SurroundedByParenthesesRegex();

    [GeneratedRegex(@"('([^']|'')*'|\([^)]*\)|\S+)")]
    private static partial Regex ExpressionRegex();

    [GeneratedRegex(@"^'(.*)'$")]
    private static partial Regex QuotesRegex();

    [GeneratedRegex(@"(\|\d+\|)")]
    private static partial Regex ExpressionKeysRegex();


    [GeneratedRegex(@"\$skip=(\d+)", RegexOptions.IgnoreCase)]
    private static partial Regex SkipRegex();

    [GeneratedRegex(@"\$top=(\d+)", RegexOptions.IgnoreCase)]
    private static partial Regex TopRegex();

    [GeneratedRegex(@"\$count=(true|false)", RegexOptions.IgnoreCase)]
    private static partial Regex CountRegex();
}
