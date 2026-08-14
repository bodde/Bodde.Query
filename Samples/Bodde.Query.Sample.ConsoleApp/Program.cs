using Bodde.Query.Abstractions.Models;
using Bodde.Query.Samples.Data;

var departments = DataSeeder.SeedDepartments();
var data = DataSeeder.SeedEmployees(departments).AsQueryable();

var queryToolkit = new DefaultQueryToolkit();

var employeesNamedJohn = data
    .AsQueryableWithCriteria(queryToolkit)
    .Filter("Name startswith 'John'")
    .ToResult();

ShowResults("Employees named John", employeesNamedJohn);

void ShowResults(string title, QueryCriteriaResult<Employee> result)
{
    Console.Write(title);
    if(result.Criteria.Paging?.TotalCount == true && result.TotalCount.HasValue)
    {
        Console.Write($" ({result.Items.Length}/{result.TotalCount.Value})");
    }

    Console.WriteLine(":");
    Console.WriteLine();

    Console.WriteLine(result.Items.ToDisplayTable(_ => _.Id, _ => _.Name));
}
