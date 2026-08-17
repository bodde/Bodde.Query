using Bodde.Query.Core;
using Bodde.Query.EntityFrameworkCore;
using Samples.Common;
using Microsoft.EntityFrameworkCore;

using var ctx = new CompanyDbContext();

await InitializeDatabaseAsync(ctx);

var queryToolkit = QueryToolkit.Default(executor: new EntityFrameworkCoreQueryExecutor());

var testExecutor = new QueryTester(ctx.Employees, _ => Console.WriteLine(_), queryToolkit);
await testExecutor.ExecuteAsync();

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