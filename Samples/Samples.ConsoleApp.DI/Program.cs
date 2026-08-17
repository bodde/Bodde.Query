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
        var testExecutor = new QueryTester(employees, _ => Console.WriteLine(_), queryToolkit);

        await testExecutor.ExecuteAsync();

        hostApplicationLifetime.StopApplication();
    }
}

