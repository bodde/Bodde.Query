using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Samples.Common;

public static class QueryCriteriaResultExtensions
{
    public static void OutputTo(this QueryCriteriaResult<Employee> result, Action<string> outputMethod, IQueryToolkit queryToolkit)
    {
        outputMethod($"{result.Name}:");
        outputMethod(result.Items.ToDisplayTable(
            new(_ => _.Id),
            new(_ => _.Name),
            new(_ => _.Department!.Name, header: "Department"),
            new(_ => _.Salary, valueFormatter: value => $"{value:C}"),
            new(_ => _.HireDate, header: "Hire Date", valueFormatter: value => $"{value:yyyy-MM-dd}"),
            new(_ => _.IsActive, header: "Is Active")
            ));

        if (result.TotalCount.HasValue)
        {
            outputMethod($"Total Count: {result.TotalCount.Value}");
        }

        var formattedQuery = queryToolkit.Formatter.Format(result.Criteria);
        outputMethod($"Formatted Query: {formattedQuery}");

        outputMethod(string.Empty);
    }
}