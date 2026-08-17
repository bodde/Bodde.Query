using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;
using Bodde.Query.Core;
using Samples.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Bodde.Query.Abstractions.Extensions;


var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();

builder.Services.AddQueryServices();
builder.Services.AddHostedService<QueryTesterService>();

var host = builder.Build();
host.Run();

class QueryTesterService(IQueryToolkit queryToolkit, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var departments = DataSeeder.SeedDepartments();
        var employees = DataSeeder.SeedEmployees(departments);
        var employeesWithCriteria = employees
            .AsQueryable()
            .WithCriteria(name: "Employees", queryToolkit);

        var page1 = employeesWithCriteria
            .WithPaging(skip: 0, top: 10)
            .WithName("Employees - Page 1");

        ShowResult(await page1.ToResultAsync());

        var page2 = employeesWithCriteria
            .WithPaging(skip: 10, top: 10)
            .WithName("Employees - Page 2");

        ShowResult(await page2.ToResultAsync());

        var page3 = employeesWithCriteria
            .WithPaging(skip: 20, top: 10)
            .WithName("Employees - Page 3");

        ShowResult(await page3.ToResultAsync());

        var page4 = employeesWithCriteria
            .WithPaging(skip: 30, top: 10)
            .WithName("Employees - Page 4");

        ShowResult(await page4.ToResultAsync());

        var employeesNamedJohn = employeesWithCriteria
            .WithFilter("Name startswith 'John'")
            .WithName("Employees named John");
        ShowResult(await employeesNamedJohn.ToResultAsync());

        var employeesByHireDateDesc = employeesWithCriteria
            .WithOrderBy("HireDate desc")
            .WithName("Employees by hire date (descending)");
        ShowResult(await employeesByHireDateDesc.ToResultAsync());

        var employeesFromHR = employeesWithCriteria
            .WithFilter("Department.Name eq 'Human Resources'")
            .WithName("Employees from HR");
        ShowResult(await employeesFromHR.ToResultAsync());

        var employeesFromHRByHireDateDesc = employeesFromHR
            .WithOrderBy("HireDate desc")
            .WithName("Employees from HR by hire date (descending)");
        ShowResult(await employeesFromHRByHireDateDesc.ToResultAsync());

        var employeesFromHRByHireDateDescPage2 = employeesFromHRByHireDateDesc
            .WithPaging(skip: 10, top: 10)
            .WithName("Employees from HR by hire date (descending) - Page 2");
        ShowResult(await employeesFromHRByHireDateDescPage2.ToResultAsync());

        var multipleCriteria = employeesWithCriteria
            .WithFilter("Department.Name in ('Human Resources', 'Engineering') or Salary gt 30000")
            .WithFilter("HireDate ge 2021-01-01 and IsActive eq true")   // filters can be combined using multiple WithFilter calls
            .WithOrderBy("Department.Name desc, HireDate")
            .WithPaging(skip: 0, top: 10)
            .WithName("Multiple criteria");

        ShowResult(await multipleCriteria.ToResultAsync());

        hostApplicationLifetime.StopApplication();
    }

    private void ShowResult(QueryCriteriaResult<Employee> result)
    {
        Console.WriteLine($"{result.Name}:");
        Console.WriteLine(result.Items.ToDisplayTable(
            new(_ => _.Id),
            new(_ => _.Name),
            new(_ => _.Department!.Name, header: "Department"),
            new(_ => _.Salary, valueFormatter: value => $"{value:C}"),
            new(_ => _.HireDate, header: "Hire Date", valueFormatter: value => $"{value:yyyy-MM-dd}"),
            new(_ => _.IsActive, header: "Is Active")
            ));

        if (result.TotalCount.HasValue)
        {
            Console.WriteLine($"Total Count: {result.TotalCount.Value}");
        }

        var formattedQuery = queryToolkit.Formatter.Format(result.Criteria);
        Console.WriteLine($"Formatted Query: {formattedQuery}");

        Console.WriteLine();
    }
}

