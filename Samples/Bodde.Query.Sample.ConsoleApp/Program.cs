using Bodde.Query.Abstractions.Models;
using Bodde.Query.Samples.Data;

var queryToolkit = new DefaultQueryToolkit();
var departments = DataSeeder.SeedDepartments();
var employees = DataSeeder.SeedEmployees(departments).AsQueryable();
var employeesWithCriteria = employees.AsQueryableWithCriteria("Employees", queryToolkit);

Paginate(employeesWithCriteria, page: 1, pageSize: 10);

void Paginate(QueryableWithCriteria<Employee> employeesWithCriteria, int page, int pageSize)
{
    while(true)
    {
        var skip = (page - 1) * pageSize;
        var employeesPage = employeesWithCriteria
            .Paging(skip: skip, top: pageSize)
            .ToResult();
        
        ShowResults(employeesPage);

        if (skip + pageSize >= employeesPage.TotalCount!)
            break;

        page++;
    }
}

var employeesNamedJohn = employees
    .AsQueryableWithCriteria("Employees named John", queryToolkit)
    .Filter("Name startswith 'John'")
    .ToResult();

ShowResults(employeesNamedJohn);

void ShowResults(QueryCriteriaResult<Employee> result)
{
    Console.Write(result.Name);
    Console.WriteLine(":");

    Console.WriteLine(result.Items.ToDisplayTable(_ => _.Id, _ => _.Name, _ => _.Department!.Name));
}
