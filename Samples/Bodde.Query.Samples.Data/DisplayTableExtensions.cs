using System.Linq.Expressions;
using System.Text;

namespace Bodde.Query.Samples.Data;

public static class DisplayTableExtensions
{
    public class DisplayTableColumn<T>
    {
        public DisplayTableColumn(Expression<Func<T, object>> columnSelector, string? header = null, Func<object?, string>? valueFormatter = null)
        {
            Header = header ?? GetPropertyName(columnSelector);
            ValueSelector = columnSelector.AddTestForNull()!.Compile();
            ValueFormatter = valueFormatter;

            var propertyType = GetPropertyType(columnSelector);

            RightAlign = propertyType == typeof(int) || propertyType == typeof(decimal) || propertyType == typeof(double) || propertyType == typeof(float);
        }

        public string Header { get; set; }
        public Func<object?, string>? ValueFormatter { get; set; }
        public Func<T, object> ValueSelector { get; set; }

        public bool RightAlign { get; set; }
    }

    public static string ToDisplayTable<T>(this IEnumerable<T> items, params DisplayTableColumn<T>[] columnSelectors)
    {
        var columnCount = columnSelectors.Length;
        if (columnCount == 0)
            return "Please select at least one column to display";

        var rows = items
            .Select(item => GetRowValues(item, columnSelectors))
            .ToArray();

        var columnLengths = columnSelectors.Select((col, columnIndex) => Math.Max(col.Header.Length, GetMaxColumnLength(rows, columnIndex))).ToArray();

        var sb = new StringBuilder();
        var rowLength = 0;
        var spacing = 2;
        for (var colIndex = 0; colIndex < columnCount; colIndex++)
        {
            var header = columnSelectors[colIndex].Header;
            var columnLengthWithSpacing = columnLengths[colIndex] + spacing;
            rowLength += columnLengthWithSpacing;

            sb.Append(header.PadRight(columnLengthWithSpacing));
        }
        sb.AppendLine();

        sb.AppendLine(new string('-', rowLength));

        for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
        {
            var row = rows[rowIndex];
            for (var colIndex = 0; colIndex < columnCount; colIndex++)
            {
                var value = row[colIndex];
                var columnLength = columnLengths[colIndex];

                var rightAlign = columnSelectors[colIndex].RightAlign;
                var formattedValue = rightAlign ? value.PadLeft(columnLength) : value.PadRight(columnLength);

                sb.Append(formattedValue);
                sb.Append(new string(' ', spacing));
            }

            if (rowIndex < rows.Length - 1)
            {
                sb.AppendLine();
            }
        }

        if (rows.Length == 0)
            sb.AppendLine("No data to display");

        return sb.ToString();
    }

    private static int GetMaxColumnLength(string[][] values, int columnIndex)
    {
        var columnValues = GetColumnValues(values, columnIndex);
        if (columnValues.Length == 0)
            return 0;

        return columnValues.Max(v => v.Length);
    }

    private static string[] GetColumnValues(string[][] values, int columnIndex)
    {
        return [.. values.Select(row => row[columnIndex])];
    }

    private static string[] GetRowValues<T>(T item, DisplayTableColumn<T>[] columnSelectors)
    {
        return [.. columnSelectors.Select(col => GetPropertyValue(item, col.ValueSelector, col.ValueFormatter))];
    }

    private static string GetPropertyValue<T>(T item, Func<T, object> valueGetter, Func<object?, string>? valueFormatter = null)
    {
        var value = item == null ? null : valueGetter(item);
        return valueFormatter?.Invoke(value) ?? value?.ToString() ?? "<null>";
    }

    private static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var memberExpressions = GetMemberExpressions(expression);

        return String.Join(".", memberExpressions.Select(me => me.Member.Name));
    }

    private static Type GetPropertyType<T>(Expression<Func<T, object>> expression)
    {
        var memberExpressions = GetMemberExpressions(expression);

        return memberExpressions.Last().Type;
    }

    private static MemberExpression[] GetMemberExpressions<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is UnaryExpression unaryExpression)
        {
            return [(MemberExpression)unaryExpression.Operand];
        }

        if (expression.Body is MemberExpression memberExpression)
        {
            var memberExpressions = new List<MemberExpression>();
            var innerExpression = memberExpression.Expression as MemberExpression;
            while (innerExpression != null)
            {
                memberExpressions.Add(innerExpression);
                innerExpression = innerExpression.Expression as MemberExpression;
            }

            memberExpressions.Add(memberExpression);

            return memberExpressions.ToArray();
        }

        throw new ArgumentException("Expression is not a property access expression");
    }

}


public static class ExpressionExtensions
{
    public static ExpressionType? AddTestForNull<ExpressionType>(this ExpressionType orig) 
        where ExpressionType : Expression 
        => (ExpressionType?)new NullTestVisitor().Visit(orig);
}

public static class TypeExtensions
{
    public static bool IsNullable(this Type type)
        => type.IsClass || Nullable.GetUnderlyingType(type) != null;
}

/// <summary>
/// ExpressionVisitor to replace a obj.member Expression with a null test ((obj == null) ? null : obj.member)
/// </summary>
public class NullTestVisitor : ExpressionVisitor
{
    public override Expression? Visit(Expression? node)
    {
        if (node is MemberExpression nme && nme.Expression != null && nme.Type.IsNullable())
        {
            var nullTestExpression = Expression.MakeBinary(ExpressionType.Equal, nme.Expression, Expression.Constant(null, nme.Expression.Type));

            return Expression.Condition(nullTestExpression, Expression.Constant(null, nme.Type), nme);
        }

        return base.Visit(node);
    }
}