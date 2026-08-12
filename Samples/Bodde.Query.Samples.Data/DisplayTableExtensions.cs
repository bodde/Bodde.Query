using System.Linq.Expressions;
using System.Text;

public static class DisplayTableExtensions
{
    public static string ToDisplayTable<T>(this IEnumerable<T> items, params Expression<Func<T, object>>[] columnSelectors)
    {
        var columnCount = columnSelectors.Length;
        if (columnCount == 0)
            return "Please select at least one column to display";


        var headers = columnSelectors.Select(selector => GetPropertyName(selector)).ToArray();
        var columnValueSelectors = columnSelectors.Select(_ => _.Compile()).ToArray();

        var rows = items
            .Select(item => GetRowValues(item, columnValueSelectors))
            .ToArray();

        var columnLengths = headers.Select((header, columnIndex) => Math.Max(header.Length, GetMaxColumnLength(rows, columnIndex))).ToArray();

        var sb = new StringBuilder();
        var rowLength = 0;
        var spacing = 1;
        for (var colIndex = 0; colIndex < columnCount; colIndex++)
        {
            var header = headers[colIndex];
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
                var columnLengthWithSpacing = columnLengths[colIndex] + spacing;

                sb.Append(value.PadRight(columnLengthWithSpacing));
            }

            sb.AppendLine();
        }

        if(rows.Length == 0)
            sb.AppendLine("No data to display");

        return sb.ToString();
    }

    private static int GetMaxColumnLength(string[][] values, int columnIndex)
    {
        var columnValues = GetColumnValues(values, columnIndex);
        if(columnValues.Length == 0)
            return 0;

        return columnValues.Max(v => v.Length);
    }

    private static string[] GetColumnValues(string[][] values, int columnIndex)
    {
        return [.. values.Select(row => row[columnIndex])];
    }

    private static string[] GetRowValues<T>(T item, Func<T, object>[] columnValueSelectors)
    {
        return [.. columnValueSelectors.Select(selector => GetPropertyValue(item, selector))];
    }

    private static string GetPropertyValue<T>(T item, Func<T, object> valueGetter)
    {
        return valueGetter(item)?.ToString() ?? "<null>";
    }

    private static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var memberExpression = GetMemberExpression(expression);

        return memberExpression.Member.Name;
    }

    private static MemberExpression GetMemberExpression<T, TProperty>(Expression<Func<T, TProperty>> expression)
    {
        if(expression.Body is UnaryExpression unaryExpression)
        {
            return (MemberExpression)unaryExpression.Operand;
        }

        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression;
        }

        throw new ArgumentException("Expression is not a property access expression");
    }
}