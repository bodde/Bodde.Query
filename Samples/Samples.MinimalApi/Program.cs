using System.Text;
using Bodde.Query.Abstractions.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;
using Bodde.Query.Core;
using Bodde.Query.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Samples.Common;
using Samples.Common.EFCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CompanyDbContext>();
builder.Services.AddQueryServices(_ => _.WithEntityFrameworkCore());
builder.Services.AddScoped<EmployeesService>();

builder.Services.AddOpenApi();
builder.Services.AddHostedService<InitializeDatabaseService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt => opt
        .WithClassicLayout()
        .HideSearch()
        .HideSidebar()
        .ExpandAllModelSections()
    );

    // redirect home to scalar
    app
        .MapGet("/", () => Results.Redirect("/scalar/#tag/samplesminimalapi/GET/employees"))
        .ExcludeFromDescription();
}

app.MapGet("/employees", async (    
    [AsParameters]QueryCriteriaParameters queryCriteria,
    EmployeesService service
    ) => 
    {
        try
        {       
            var result = await service.GetAsync(queryCriteria);
            return Results.Ok(result);
        }
        catch(FormatException formatEx)
        {
            // query parsing gone wrong
            return Results.BadRequest(formatEx.Message);
        }
        catch(Exception ex)
        {
            app.Logger.LogError(ex, ex.Message);
            return Results.InternalServerError();
        }
    }
    );

app.Run();

internal class EmployeesService(
    CompanyDbContext ctx,
    IQueryToolkit queryToolkit,
    ILogger<EmployeesService> logger
    )
{
    internal async Task<QueryCriteriaResult<Employee>> GetAsync(QueryCriteriaParameters queryCriteriaParameters)
    {
        Log(queryCriteriaParameters);

        var queryCriteria = queryToolkit.Parser.Parse(queryCriteriaParameters);

        var result = await ctx.Employees
            .Include(_ => _.Department) // use dto mapping/projection for real projects
            .WithCriteria("Get employees", queryCriteria, queryToolkit).ToResultAsync();

        return result;
    }

    private void Log(QueryCriteriaParameters queryCriteriaParameters)
    {
        var nd = "<nd>";
        var filter = queryCriteriaParameters.Filter ?? nd;
        var orderBy = queryCriteriaParameters.OrderBy ?? nd;
        var skip = queryCriteriaParameters.Skip?.ToString() ?? nd;
        var top = queryCriteriaParameters.Top.ToString() ?? nd;
        var count = queryCriteriaParameters.Count ?? false;

        var sb = new StringBuilder();

        sb.AppendLine("\nGet Employees");
        sb.AppendLine($"Filter:\t\t{filter}");
        sb.AppendLine($"OrderBy:\t{orderBy}");
        sb.AppendLine($"Skip:\t\t{skip}");
        sb.AppendLine($"Top:\t\t{top}");
        sb.AppendLine($"Count:\t\t{count}");

        logger.LogInformation(sb.ToString());
    }
}

internal class InitializeDatabaseService(
    IServiceScopeFactory serviceScopeFactory, 
    ILogger<InitializeDatabaseService> logger
    ) : BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Initializing database {dbPath}", CompanyDbContext.DbPath);

        using var scope = serviceScopeFactory.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<CompanyDbContext>();

        await ctx.EnsureRecreatedAsync(stoppingToken);
    }
}
