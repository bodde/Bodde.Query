using Microsoft.EntityFrameworkCore;

namespace Samples.Common.EFCore;

public class CompanyDbContext : DbContext
{

    public static string DbPath => Path.Combine(Path.GetTempPath(), "companies.db");

    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }

    public async Task EnsureRecreatedAsync(CancellationToken ct = default)
    {
        await Database.EnsureDeletedAsync(ct);
        await Database.EnsureCreatedAsync(ct);
    }

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
