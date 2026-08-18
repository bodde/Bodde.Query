using Bodde.Query.Core;
using Bodde.Query.EntityFrameworkCore;
using Samples.Common;
using Samples.Common.EFCore;


using var ctx = new CompanyDbContext();

Console.WriteLine($"Initializing database {CompanyDbContext.DbPath}");
await ctx.EnsureRecreatedAsync();

var queryToolkit = QueryToolkit.Default(executor: new EntityFrameworkCoreQueryExecutor());

var testExecutor = new QueryTester(ctx.Employees, _ => Console.WriteLine(_), queryToolkit);
await testExecutor.ExecuteAsync();
