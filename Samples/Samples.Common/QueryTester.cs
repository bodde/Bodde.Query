using Bodde.Query.Abstractions.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;

using QueryComposer = System.Func<Bodde.Query.Abstractions.Models.QueryableWithCriteria<Samples.Common.Employee>, Bodde.Query.Abstractions.Models.QueryableWithCriteria<Samples.Common.Employee>>;

namespace Samples.Common;

public class QueryTester(IEnumerable<Employee> employees, Action<string> outputMethod, IQueryToolkit queryToolkit)
{
    // paging
    private static readonly QueryComposer page1 = _ => _.WithName("Employees - Page 1").WithPaging(skip: 0, top: 10);
    private static readonly QueryComposer page2 = _ => _.WithName("Employees - Page 2").WithPaging(skip: 10, top: 10);
    private static readonly QueryComposer page3 = _ => _.WithName("Employees - Page 3").WithPaging(skip: 20, top: 10);
    private static readonly QueryComposer page4 = _ => _.WithName("Employees - Page 4").WithPaging(skip: 30, top: 10);

    // filter

    private static readonly QueryComposer active = _ => _.WithName("Active employees").WithFilter("IsActive eq true");
    private static readonly QueryComposer namedJohn = _ => _.WithName("Employees named John").WithFilter("Name startswith 'John'");
    private static readonly QueryComposer fromHR = _ => _.WithName("Employees named John").WithFilter("Department.Name eq 'Human Resources'");

    // orderby
    private static readonly QueryComposer byHireDateDesc = _ => _.WithName("Employees by hire date (descending)").WithOrderBy("HireDate desc");

    // combined
    private static readonly QueryComposer fromHRByHireDateDesc = _ =>
        byHireDateDesc(fromHR(_))
        .WithName("Employees from HR by hire date (descending)");

    private static readonly QueryComposer fromHRByHireDateDescPage2 = _ =>
        page2(byHireDateDesc(fromHR(_)))
        .WithName("Employees from HR by hire date (descending) - Page 2");

    private static readonly QueryComposer activeFromHRByHireDateDesc = _ =>
          byHireDateDesc(fromHR(active(_)))
          .WithName("Active employees from HR by hire date (descending)");

    // multiple
    private static readonly QueryComposer multipleCriteria = _ => _
        .WithFilter("Department.Name in ('Human Resources', 'Engineering') or Salary gt 30000")
        .WithFilter("HireDate ge 2021-01-01 and IsActive eq true")   // filters can be combined using multiple WithFilter calls
        .WithOrderBy("Department.Name desc, HireDate")
        .WithPaging(skip: 0, top: 10)
        .WithName("Multiple criteria");

    private static QueryComposer[] queryComposers = [
        page1,
        page2,
        page3,
        page4,
        active,
        namedJohn,
        fromHR,
        byHireDateDesc,
        fromHRByHireDateDesc,
        fromHRByHireDateDescPage2,
        activeFromHRByHireDateDesc,
        multipleCriteria
    ];

    public void Execute()
    {
        var employeesWithCriteria = CreateEmployeesWithCriteria();

        foreach (var queryComposer in queryComposers)
        {
            var query = queryComposer(employeesWithCriteria);
            var result = query.ToResult();

            result.OutputTo(outputMethod, queryToolkit);
        }
    }

    public async Task ExecuteAsync()
    {
        var employeesWithCriteria = CreateEmployeesWithCriteria();

        foreach (var queryComposer in queryComposers)
        {
            var query = queryComposer(employeesWithCriteria);
            var result = await query.ToResultAsync();

            result.OutputTo(outputMethod, queryToolkit);
        }
    }

    private QueryableWithCriteria<Employee> CreateEmployeesWithCriteria()
    {
        return employees
            .AsQueryable()
            .WithCriteria(name: "Employees", queryToolkit);
    }
}