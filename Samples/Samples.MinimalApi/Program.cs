using System.Text;
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
    [AsParameters]QueryCriteriaParams queryCriteriaParams,
    CompanyDbContext ctx,
    IQueryToolkit queryToolkit
    ) => 
    {
        try
        {       
            var queryCriteria = queryToolkit.Parser.Parse(queryCriteriaParams);

            var result = await ctx.Employees
                .Include(_ => _.Department) // use dto mapping/projection for real projects
                .WithCriteria("Get employees", queryCriteria, queryToolkit)
                .ToResultAsync();

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
