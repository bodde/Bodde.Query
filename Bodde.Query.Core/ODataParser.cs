using System.Globalization;
using System.Text.RegularExpressions;
using Bodde.Common.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Bodde.Query.Core;

internal class ODataParser : IQueryCriteriaParser
{
    public QueryCriteria Parse(string criteriaString)
    {
        if (criteriaString == null)
            throw new ArgumentNullException(nameof(criteriaString));

        var paging = ParsePaging(criteriaString);

        var parts = criteriaString.Tokenize('&');

        var filterCriteriaString = parts.FirstOrDefault(_ => _.StartsWith("$filter=", StringComparison.OrdinalIgnoreCase));
        var filter = filterCriteriaString != null ? ParseFilter(filterCriteriaString) : null;

        var orderByCriteriaString = parts.FirstOrDefault(_ => _.StartsWith("$orderby=", StringComparison.OrdinalIgnoreCase));
        var orderBy = orderByCriteriaString != null ? ParseOrderBy(orderByCriteriaString) : null;

        return new QueryCriteria(Filter: filter, OrderBy: orderBy, Paging: paging);
    }

    public QueryCriteria Parse(string? filter = null, string? orderBy = null, int? skip = null, int? top = null, bool? totalCount = null)
    {
        return Parse(new QueryCriteriaParams
        {
            Filter = filter,
            OrderBy = orderBy,
            Skip = skip,
            Top = top,
            Count = totalCount
        });
    }

    public QueryCriteria Parse(QueryCriteriaParams queryCriteriaParameters)
    {
        if (queryCriteriaParameters == null)
            throw new ArgumentNullException(nameof(queryCriteriaParameters));

        var paging = new PagingCriteria(
            Skip: queryCriteriaParameters.Skip,
            Top: queryCriteriaParameters.Top,
            TotalCount: queryCriteriaParameters.Count
        );

        var filter = queryCriteriaParameters.Filter != null ? ParseFilter(queryCriteriaParameters.Filter) : null;
        var orderBy = queryCriteriaParameters.OrderBy != null ? ParseOrderBy(queryCriteriaParameters.OrderBy) : null;

        return new QueryCriteria(
            Paging: paging,
            Filter: filter,
            OrderBy: orderBy
        );
    }


    public PagingCriteria ParsePaging(string pagingString)
    {
        if (pagingString == null)
            throw new ArgumentNullException(nameof(pagingString));

        var skipValue = SkipRegex.Match(pagingString).Groups[1].Value;
        var topValue = TopRegex.Match(pagingString).Groups[1].Value;
        var countValue = CountRegex.Match(pagingString).Groups[1].Value;

        return new PagingCriteria(
            Skip: string.IsNullOrEmpty(skipValue) ? null : int.Parse(skipValue),
            Top: string.IsNullOrEmpty(topValue) ? null : int.Parse(topValue),
            TotalCount: string.IsNullOrEmpty(countValue) ? null : bool.Parse(countValue)
        );
    }

    public FilterCriteria.FilterExpression ParseFilterExpression(string filterString)
    {
        if (filterString == null)
            throw new ArgumentNullException(nameof(filterString));

        if (filterString.StartsWith("$filter=", StringComparison.OrdinalIgnoreCase))
            filterString = filterString.Substring("$filter=".Length);

        filterString = filterString.Trim();

        var expressionsBag = new Dictionary<string, FilterCriteria.FilterExpression>();

        filterString = ProcessComparisonExpressions(filterString, expressionsBag);
        filterString = ProcessNotAndLogicalExpressions(filterString, expressionsBag);

        return CreateTopLevelExpression(filterString, expressionsBag);
    }

    public FilterCriteria ParseFilter(string filterString)
    {
        var topLevelExpression = ParseFilterExpression(filterString);
        return new FilterCriteria(topLevelExpression);
    }

    public OrderByCriteria ParseOrderBy(string orderByString)
    {
        if (orderByString == null)
            throw new ArgumentNullException(nameof(orderByString));

        if (orderByString.StartsWith("$orderby=", StringComparison.OrdinalIgnoreCase))
            orderByString = orderByString.Substring("$orderby=".Length);

        orderByString = orderByString.Trim();

        var orderByItems = orderByString
            .Tokenize(',')
            .Select(_ => ParseOrderByItem(_))
            .ToArray();

        return new OrderByCriteria(orderByItems);
    }

    private OrderByCriteria.OrderByItem ParseOrderByItem(string itemString)
    {
        if (itemString == null)
            throw new ArgumentNullException(nameof(itemString));

        var parts = itemString.Tokenize(' ');
        var propertyPath = parts[0];
        var direction = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? OrderByCriteria.SortDirection.Descending
            : OrderByCriteria.SortDirection.Ascending;

        return new OrderByCriteria.OrderByItem(
            PropertyPath: propertyPath,
            Direction: direction
        );
    }


    private string ProcessComparisonExpressions(string filterString, Dictionary<string, FilterCriteria.FilterExpression> expressionsBag)
    {
        var comparisonStatements = GetComparisonStatements(filterString);

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
        var notMatches = NotExpressionsRegex.Matches(filterString);
        foreach (Match notMatch in notMatches)
        {
            var notStatement = notMatch.Value;

            string innerExpressionKey = GetInnerExpressionKey(notMatch.Groups);
            var innerExpression = expressionsBag[innerExpressionKey];

            var notExpression = new FilterCriteria.NotExpression(innerExpression);

            var notExpressionKey = NextExpressionKey(expressionsBag);
            expressionsBag[notExpressionKey] = notExpression;

            filterString = filterString.Replace(notStatement, notExpressionKey);
        }

        return filterString;
    }

    private static string GetInnerExpressionKey(GroupCollection notMatchGroups)
    {            
        var notMatchKey1Group = notMatchGroups["key1"];
        if(notMatchKey1Group != null && notMatchKey1Group.Success)
            return notMatchKey1Group.Value;

        var notMatchKey2Group = notMatchGroups["key2"];
        if(notMatchKey2Group != null && notMatchKey2Group.Success)
            return notMatchKey2Group.Value;

        throw new InvalidOperationException("No valid group found for not expression.");  
    }

    private static string[] GetInnerLogicalExpressions(string filterString)
    {
        return SurroundedByParenthesesRegex.Matches(filterString)
            .Cast<Match>()
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
            var expression = innerExpression.Substring(1, innerExpression.Length - 2); // remove surrounding parentheses

            var logicalExpression = CreateLogicalExpression(expression, expressionsBag);

            var logicalExpressionKey = NextExpressionKey(expressionsBag);
            expressionsBag[logicalExpressionKey] = logicalExpression;

            filterString = filterString.Replace(innerExpression, logicalExpressionKey);
        }
        return filterString;
    }

    private FilterCriteria.FilterExpression CreateTopLevelExpression(string filterString, Dictionary<string, FilterCriteria.FilterExpression> expressionsBag)
    {
        if (LogicalOperatorsRegex.IsMatch(filterString))
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
            throw new InvalidOperationException("At least two expressions are required to create a logical expression.");

        var expressions = expressionKeys
            .Select(key => expressionsBag[key])
            .ToArray();

        var logicalExpression = new FilterCriteria.LogicalExpression(
            Operator: logicalOperator,
            First: expressions[0],
            Second: expressions[1],
            Others: expressions.Length > 2 ? expressions.Skip(2).ToArray() : Array.Empty<FilterCriteria.FilterExpression>()
            );

        return logicalExpression;
    }

    private string[] GetExpressionKeys(string expressionString)
    {

        var expressionKeys = ExpressionKeysRegex
            .Matches(expressionString)
            .Cast<Match>()
            .Select(m => m.Value)
            .ToArray();

        return expressionKeys;
    }

    private FilterCriteria.LogicalOperator GetLogicalOperator(string filterString)
    {
        var logicalOperators = LogicalOperatorsRegex
            .Matches(filterString)
            .Cast<Match>()
            .Select(m => m.Value.Trim().ToLower())
            .ToArray();

        if (logicalOperators.Distinct().Count() > 1)
        {
            throw new FormatException("Only one logical operator per logical expression is supported.");
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
        var parts = ExpressionRegex.Matches(comparisonStatement.Trim()).Cast<Match>().Select(m => m.Value).ToArray();

        if (parts.Length != 3)
        {
            throw new NotImplementedException("Only binary comparison expressions are supported.");
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
        var comparisonStatements = ComparisonStatementsRegex
            .Matches(filterString)
            .Cast<Match>()
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
            var match = InValuesRegex.Match(valueString);
            if (!match.Success)
            {
                throw new FormatException("Invalid syntax for 'in' operator.");
            }

            var valuesPart = match.Groups[1].Value;
            var valueTypes = valuesPart.Tokenize(',')
                .Select(v => ParseValue(v))
                .ToArray();

            var types = valueTypes.Select(vt => vt.type).ToArray();
            if (types.Distinct().Count() > 1)
            {
                throw new FormatException("All values for 'in' operator must be of the same type.");
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

    internal static (object? value, Type type) ParseValue(string valueString)
    {
        if (valueString.Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return (null!, typeof(object));
        }

        var quotesRegex = QuotesRegex;
        if (quotesRegex.IsMatch(valueString))
        {
            return (quotesRegex.Replace(valueString, "$1"), typeof(string));
        }

        if (int.TryParse(valueString, out var intValue))
        {
            return (intValue, typeof(int));
        }
        if (double.TryParse(valueString, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
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

        throw new FormatException($"OData operator '{operatorString}' is not supported. Supported operators are: {_comparisonOperators.Keys.ToCsv()}");
    }

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

    private static readonly Regex ComparisonStatementsRegex = new(
        @"([\w\.]+\s+(?:\w+)\s+(?:null|true|false|'[^']*'|[\d\-T\:\.Z]+|\((?:(?:null|true|false|'[^']*'|[\d\-T\:\.Z]+)(?:\s*,\s*)?)+\)))", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex LogicalOperatorsRegex = new(
        @"\s+(and|or)\s+", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex NotExpressionsRegex = new(
        @"(?:not\s+\((?'key1'\|\d+\|)\))|(?:not\s+(?'key2'\|\d+\|))", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex SurroundedByParenthesesRegex = new(
        @"\(([^()]+)\)", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ExpressionRegex = new(
        @"('([^']|'')*'|\([^)]*\)|\S+)", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex QuotesRegex = new(
        @"^'(.*)'$", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ExpressionKeysRegex = new(
        @"(\|\d+\|)", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex SkipRegex = new(
        @"\$skip=(\d+)", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex TopRegex = new(
        @"\$top=(\d+)", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex CountRegex = new(
        @"\$count=(true|false)", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex InValuesRegex = new(
        @"\((.*)\)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);
}
