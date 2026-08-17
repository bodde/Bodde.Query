using Bodde.Query.Abstractions.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

namespace Samples.Common;

public class QueryTester(IEnumerable<Employee> employees, Action<string> outputMethod, IQueryToolkit queryToolkit)
{
    private static Func<QueryableWithCriteria<Employee>, QueryableWithCriteria<Employee>>[] tests = [
        _ => _.WithName("Employees - Page 1").WithPaging(skip: 0, top: 10),
        _ => _.WithName("Employees - Page 2").WithPaging(skip: 10, top: 10),
        _ => _.WithName("Employees - Page 3").WithPaging(skip: 20, top: 10),
        _ => _.WithName("Employees - Page 4").WithPaging(skip: 30, top: 10),
    ];

    public void Execute()
    {
        var employeesWithCriteria = CreateEmployeesWithCriteria();

        var page1 = employeesWithCriteria
            .WithPaging(skip: 0, top: 10)
            .WithName("Employees - Page 1");

        page1.ToResult().OutputTo(outputMethod, queryToolkit);

        var page2 = employeesWithCriteria
            .WithPaging(skip: 10, top: 10)
            .WithName("Employees - Page 2");

        page2.ToResult().OutputTo(outputMethod, queryToolkit);

        var page3 = employeesWithCriteria
            .WithPaging(skip: 20, top: 10)
            .WithName("Employees - Page 3");

        page3.ToResult().OutputTo(outputMethod, queryToolkit);

        var page4 = employeesWithCriteria
            .WithPaging(skip: 30, top: 10)
            .WithName("Employees - Page 4");

        page4.ToResult().OutputTo(outputMethod, queryToolkit);

        var employeesNamedJohn = employeesWithCriteria
            .WithFilter("Name startswith 'John'")
            .WithName("Employees named John");
        employeesNamedJohn.ToResult().OutputTo(outputMethod, queryToolkit);

        var employeesByHireDateDesc = employeesWithCriteria
            .WithOrderBy("HireDate desc")
            .WithName("Employees by hire date (descending)");
        employeesByHireDateDesc.ToResult().OutputTo(outputMethod, queryToolkit);

        var employeesFromHR = employeesWithCriteria
            .WithFilter("Department.Name eq 'Human Resources'")
            .WithName("Employees from HR");
        employeesFromHR.ToResult().OutputTo(outputMethod, queryToolkit);

        var employeesFromHRByHireDateDesc = employeesFromHR
            .WithOrderBy("HireDate desc")
            .WithName("Employees from HR by hire date (descending)");
        employeesFromHRByHireDateDesc.ToResult().OutputTo(outputMethod, queryToolkit);

        var employeesFromHRByHireDateDescPage2 = employeesFromHRByHireDateDesc
            .WithPaging(skip: 10, top: 10)
            .WithName("Employees from HR by hire date (descending) - Page 2");
        employeesFromHRByHireDateDescPage2.ToResult().OutputTo(outputMethod, queryToolkit);

        var multipleCriteria = employeesWithCriteria
            .WithFilter("Department.Name in ('Human Resources', 'Engineering') or Salary gt 30000")
            .WithFilter("HireDate ge 2021-01-01 and IsActive eq true")   // filters can be combined using multiple WithFilter calls
            .WithOrderBy("Department.Name desc, HireDate")
            .WithPaging(skip: 0, top: 10)
            .WithName("Multiple criteria");
        multipleCriteria.ToResult().OutputTo(outputMethod, queryToolkit);
    }

    private Bodde.Query.Abstractions.Models.QueryableWithCriteria<Employee> CreateEmployeesWithCriteria()
    {
        return employees
            .AsQueryable()
            .WithCriteria(name: "Employees", queryToolkit);
    }
}