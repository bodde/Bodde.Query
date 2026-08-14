using Bodde.Common.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Samples.Data;
using static Bodde.Query.Abstractions.Models.FilterCriteria;

var departments = DataSeeder.SeedDepartments();
var data = DataSeeder.SeedEmployees(departments).AsQueryable();
var filteredData = data;

var queryToolkit = new QueryToolkit();


var employeesNamedJohn = queryToolkit.Parser.Parse(filter: "Name startswith 'John'");
var filteredData1 = await queryToolkit.Handler.ToResultAsync(data, employeesNamedJohn);

var commands = new (string name, Func<QueryCriteria, QueryCriteria> updateCriteria)[]
{
    ( "Reset", ResetCriteriaCommand),
    ( "Add filter", AddFilterCommand ),
    ( "Add order by", AddOrderByCommand )
};

var queryCriteria = new QueryCriteria();

while (true)
{
    var displayTable = filteredData.ToDisplayTable(d => d.Id, d => d.Name, d => d.Department!.Name, d => d.Salary, d => d.HireDate, d => d.IsActive);   
    Console.WriteLine(displayTable);

    var queryCriteriaString = queryToolkit.Formatter.Format(queryCriteria);
    queryCriteriaString = queryCriteriaString.IsEmpty() ? "(none)" : queryCriteriaString;
    Console.WriteLine($"Current query criteria: {queryCriteriaString}");

    Console.WriteLine("Select command (or press Enter to exit)");
    for (int i = 0; i < commands.Length; i++)
    {
        var command = commands[i];
        Console.WriteLine($"{i}. {command.name}");
    }

    var input = Console.ReadLine() ?? string.Empty;
    if (input.IsEmpty())
        break;

    if (int.TryParse(input, out int commandIndex) && commandIndex >= 0 && commandIndex < commands.Length)
    {
        var command = commands[commandIndex];
        Console.WriteLine($"{command.name}: ");
        try
        {
            var newQueryCriteria = command.updateCriteria(queryCriteria);
            filteredData = queryToolkit.Handler.ApplyCriteria(data, newQueryCriteria);

            queryCriteria = newQueryCriteria;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
    }
}

QueryCriteria ResetCriteriaCommand(QueryCriteria criteria) => new();

QueryCriteria AddFilterCommand(QueryCriteria queryCriteria)
{
    var input = Console.ReadLine() ?? string.Empty;
    if(input.IsEmpty())
        return queryCriteria;

    var filterExpression = queryToolkit.Parser.ParseFilterExpression(input);
    if(filterExpression is null)
        return queryCriteria;
    
    if(queryCriteria.Filter is null)
    {
        return new QueryCriteria(
            Filter: new FilterCriteria(filterExpression), 
            OrderBy: queryCriteria.OrderBy, 
            Paging: queryCriteria.Paging
            );
    }

    var combinedExpression = new LogicalExpression(
        LogicalOperator.And, 
        queryCriteria.Filter.Expression, 
        filterExpression
    );

    return new QueryCriteria(
        Filter: new FilterCriteria(combinedExpression),
        OrderBy: queryCriteria.OrderBy,
        Paging: queryCriteria.Paging
    );       
}

QueryCriteria AddOrderByCommand(QueryCriteria queryCriteria)
{           
    var input = Console.ReadLine() ?? string.Empty;
    if(input.IsEmpty())
        return queryCriteria;

    var orderByItems = queryToolkit.Parser.ParseOrderByItems(input);
    if(orderByItems is null || orderByItems.Length == 0)
        return queryCriteria;

    var criteriaOrderByItems = queryCriteria.OrderBy?.Items ?? [];
    var combinedOrderByItems = criteriaOrderByItems.Concat(orderByItems).ToArray();

    return new QueryCriteria(
        Filter: queryCriteria.Filter,
        OrderBy: new OrderByCriteria(combinedOrderByItems),
        Paging: queryCriteria.Paging
    );    
}
