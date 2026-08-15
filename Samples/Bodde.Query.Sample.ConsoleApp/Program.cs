using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Bodde.Query.Samples.Data;

var queryToolkit = new DefaultQueryToolkit();
var departments = DataSeeder.SeedDepartments();
var employees = DataSeeder.SeedEmployees(departments).AsQueryable();
var employeesWithCriteria = employees.WithCriteria("Employees", queryToolkit);

Paginate(employeesWithCriteria, page: 1, pageSize: 10);
EmployeesNamedJohn(employeesWithCriteria);

void Paginate(QueryableWithCriteria<Employee> employeesWithCriteria, int page, int pageSize)
{
    while (true)
    {
        var skip = (page - 1) * pageSize;
        var employeesPage = employeesWithCriteria
            .WithPaging(skip: skip, top: pageSize)
            .WithName($"{employeesWithCriteria.Name} - Page {page}")
            .ToResult();

        ShowResults(employeesPage);

        if (skip + pageSize >= employeesPage.TotalCount!)
            break;

        page++;
    }
}

void EmployeesNamedJohn(QueryableWithCriteria<Employee> employeesWithCriteria)
{
    var employeesNamedJohn = employeesWithCriteria
        .WithName("Employees named John")
        .WithFilter("Name startswith 'John'")
        .ToResult();

    ShowResults(employeesNamedJohn);
}

void ShowResults(QueryCriteriaResult<Employee> result)
{
    Console.Write(result.Name);
    Console.WriteLine(":");

    Console.WriteLine(result.Items.ToDisplayTable(_ => _.Id, _ => _.Name, _ => _.Department!.Name));
}
