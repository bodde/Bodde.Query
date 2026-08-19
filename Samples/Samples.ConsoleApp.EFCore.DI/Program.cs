using Bodde.Query.Core;
using Bodde.Query.EntityFrameworkCore;
using Samples.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Bodde.Query.Abstractions.Services;
using Samples.Common.EFCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();

builder.Services.AddDbContext<CompanyDbContext>();
builder.Services.AddQueryServices(_ => _.WithEntityFrameworkCore());

builder.Services.AddHostedService<QueryTesterService>();

var host = builder.Build();
host.Run();


class QueryTesterService(CompanyDbContext ctx, IQueryToolkit queryToolkit, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"Initializing database {CompanyDbContext.DbPath}");
        await ctx.EnsureRecreatedAsync(stoppingToken);

        var testExecutor = new QueryTester(ctx.Employees, _ => Console.WriteLine(_), queryToolkit);
        await testExecutor.ExecuteAsync();

        hostApplicationLifetime.StopApplication();
    }

    static async Task InitializeDatabaseAsync(CompanyDbContext ctx)
    {
        Console.WriteLine($"Initializing {CompanyDbContext.DbPath}");
        await ctx.Database.EnsureDeletedAsync();
        await ctx.Database.EnsureCreatedAsync();
    }
}
