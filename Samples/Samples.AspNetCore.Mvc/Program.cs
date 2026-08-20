using Samples.Common.EFCore;
using Bodde.Query.Core;
using Bodde.Query.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Bodde.Query.Abstractions.Services;
using Bodde.Query.Abstractions.Models;
using Samples.Common;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CompanyDbContext>();
builder.Services.AddQueryServices(_ => _.WithEntityFrameworkCore());

builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<InitializeDatabaseService>();

builder.Services.AddControllers();

var app = builder.Build();

MapOpenApi(app);

app.MapControllers();

app.Run();

static void MapOpenApi(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        // redirect home to Swagger UI
        app
            .MapGet("/", () => Results.Redirect("swagger"))
            .ExcludeFromDescription();
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


[ApiController]
[Route("employees")]
public class EmployeesController(
	CompanyDbContext context,
	IQueryToolkit queryToolkit) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<QueryCriteriaResult<Employee>>> Get(
		[FromQuery] QueryCriteriaParams queryCriteriaParams)
	{
		var queryCriteria = queryToolkit.Parser.Parse(queryCriteriaParams);

		var result = await context.Employees
			.Include(employee => employee.Department) // use dto mapping/projection for real projects
			.WithCriteria("Get employees", queryCriteria, queryToolkit)
			.ToResultAsync();

		return Ok(result);
	}
}