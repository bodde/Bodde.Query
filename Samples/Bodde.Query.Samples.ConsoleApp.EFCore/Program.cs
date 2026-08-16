using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Bodde.Query.EntityFrameworkCore;
using Bodde.Query.Samples.Data;
using Microsoft.EntityFrameworkCore;

using var ctx = new CompanyDbContext();

await InitializeDatabaseAsync(ctx);

var queryToolkit = QueryToolkit.Default(executor: new EntityFrameworkCoreQueryExecutor());

var employeesWithCriteria = ctx.Employees
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


void ShowResult(QueryCriteriaResult<Employee> result)
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

static async Task InitializeDatabaseAsync(CompanyDbContext ctx)
{
    Console.WriteLine($"Initializing {CompanyDbContext.DbPath}");
    await ctx.Database.EnsureDeletedAsync();
    await ctx.Database.EnsureCreatedAsync();
}

internal class CompanyDbContext : DbContext
{
    public static string DbPath => Path.Combine(Path.GetTempPath(), "companies.db");

    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseSqlite($"Data Source={DbPath}")
            .UseAsyncSeeding(SeedDataAsync);

    private async Task SeedDataAsync(DbContext ctx, bool _, CancellationToken ct)
    {
        var departments = DataSeeder.SeedDepartments();
        await ctx.Set<Department>().AddRangeAsync(departments, ct);

        var employees = DataSeeder.SeedEmployees(departments);
        await ctx.Set<Employee>().AddRangeAsync(employees, ct);

        await ctx.SaveChangesAsync(ct);
    }
}