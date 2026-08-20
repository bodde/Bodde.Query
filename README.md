# Bodde.Query

Bodde.Query is a set of NuGet packages for building, parsing, formatting, and executing query criteria.

It provides reusable support for filtering, sorting, and paging `IQueryable<T>` data sources.

It supports a lightweight partial, limited subset of OData query constructs, including `$filter`, `$orderby`, `$top`, `$skip`, and `$count`. These constructs can describe queries without requiring a dependency on `Microsoft.OData` or the needing EDM models generation.

The toolkit is extensible and includes integrations for Entity Framework Core.

## Table of Contents

- [Features](#features)
- [Packages](#packages)
- [Installation](#installation)
- [Usage](#usage)
	- [Getting the query toolkit](#getting-the-query-toolkit)
	- [1. Execute a query without criteria](#1-execute-a-query-without-criteria)
	- [1. Filter results](#1-filter-results)
	- [2. Name a query](#2-name-a-query)
	- [3. Filter and sort results](#3-filter-and-sort-results)
	- [4. Apply paging and request the total count](#4-apply-paging-and-request-the-total-count)
	- [5. Parse and execute a complete OData-style query asynchronously](#5-parse-and-execute-a-complete-odata-style-query-asynchronously)
	- [6. Minimal API with Entity Framework Core](#6-minimal-api-with-entity-framework-core)
	- [7. ASP.NET Core MVC with Entity Framework Core](#7-aspnet-core-mvc-with-entity-framework-core)
	- [OData-style filters reference](#odata-style-filters-reference)
	- [Minimal API with Entity Framework Core](#minimal-api-with-entity-framework-core)
	- [ASP.NET Core MVC with Entity Framework Core](#aspnet-core-mvc-with-entity-framework-core)
    - [Example projects with source code](#example-projects-with-source-code)
- [Configuration](#configuration)
	- [Default service registration](#default-service-registration)
	- [Entity Framework Core](#entity-framework-core-1)
	- [Custom implementations](#custom-implementations)
- [Compatibility](#compatibility)
- [Versioning](#versioning)
- [Contributing](#contributing)
- [License](#license)

## Features

- Lightweight, partial support for OData-style `$filter`, `$orderby`, `$top`, `$skip`, and `$count` query constructs.
- Comparison operators including `eq`, `ne`, `gt`, `ge`, `lt`, `le`, `contains`, `startswith`, `endswith`, and `in`.
- Logical filter composition with `and`, `or`, `not`, and parenthesized expressions.
- Fluent query composition through `QueryWithCriteria<T>` and LINQ `IQueryable<T>` sources.
- Synchronous and asynchronous query execution.
- Optional total-count calculation when `$count=true` or `totalCount: true` is requested.
- Entity Framework Core integration with a database-backed query executor.
- Dependency injection support with configurable service lifetimes.
- Replaceable parsers, formatters, expression builders, criteria handlers, and query executors.
- Direct usage without dependency injection through `QueryToolkit.Default()`.
- No dependency on `Microsoft.OData` or generated EDM models.
- Compatible with ASP.NET Core Minimal APIs and MVC controllers through `QueryCriteriaParams`.

## Packages

| Package | Description |
| --- | --- | --- |
| `Bodde.Query.Abstractions` | Contains the core models and service contracts used to define, parse, format, and execute query criteria. It has no external dependencies and contains no business logic and is suitable for adding to Domain layers in n-tier solutions. |
| `Bodde.Query.Core` | Provides the main query criteria implementation, including OData-inspired parsing and formatting, LINQ expression building, and dependency injection support. |
| `Bodde.Query.EntityFrameworkCore` | Adds Entity Framework Core integration for executing query criteria against EF Core data sources. |

## Installation

### Abstractions

Install the following package for core models and service contracts references:

```bash
dotnet add package Bodde.Query.Abstractions
```

### Core

Install the following package for query parse, composition and execution against LINQ:

```bash
dotnet add package Bodde.Query.Core
```

`Bodde.Query.Core` depends on `Bodde.Query.Abstractions`, so installing it also provides the abstractions package.

Register the default query services in your dependency injection container:

```csharp
services.AddQueryServices();
```

If you don't use dependency injection, you can create and use the default toolkit directly:

```csharp
IQueryToolkit queryToolkit = QueryToolkit.Default();
```

### Entity Framework Core

Install the following package for query parse, composition and execution against Entity Framework Core:

```bash
dotnet add package Bodde.Query.EntityFrameworkCore
```

`Bodde.Query.EntityFrameworkCore` depends on `Bodde.Query.Core`, so installing it also provides the core package.

When using Entity Framework Core, configure the EF Core query executor:

```csharp
services.AddQueryServices(builder => builder.WithEntityFrameworkCore());
```

If you don't use dependency injection, you can create and use the default toolkit directly with EF Core executor:

```csharp
IQueryToolkit queryToolkit = QueryToolkit.Default(executor: new EntityFrameworkCoreQueryExecutor());
```


## Usage

### Getting the query toolkit

With dependency injection, register the query services and inject `IQueryToolkit` into the class that executes the query:

```csharp

// Linq support ...
services.AddQueryServices();

// ... or EF Core support
services.AddQueryServices(builder => builder.WithEntityFrameworkCore());

public class EmployeeService(IQueryToolkit queryToolkit)
{
}
```

Without dependency injection, create the default implementation directly:

```csharp

// Linq support ...
IQueryToolkit queryToolkit = QueryToolkit.Default();

// ... or EF Core support
IQueryToolkit queryToolkit = QueryToolkit.Default(executor: new EntityFrameworkCoreQueryExecutor());

```

### 1. Execute a query without criteria

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.ToResult();

Console.WriteLine($"Returned: {result.Items.Length}");
```

`WithCriteria` returns a `QueryWithCriteria<T>` instance, which wraps the source query and the selected toolkit. This type enables the other query extension methods, such as `WithName`, `WithFilter`, `WithOrderBy`, and `WithPaging`, to be chained before execution.

### 1. Filter results

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("IsActive eq true")
	.ToResult();

Console.WriteLine(result.Criteria); // "$filter=IsActive eq true"
Console.WriteLine($"Returned: {result.Items.Length}");
```

### 2. Name a query

Use `WithName` to assign a descriptive name to a query. The name is included in the `QueryCriteriaResult`:

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithName("Active employees")
	.WithFilter("IsActive eq true")
	.ToResult();

Console.WriteLine(result.Name); // Active employees
Console.WriteLine(result.Criteria); // "$filter=IsActive eq true"
Console.WriteLine($"Returned: {result.Items.Length}");
```

### 3. Filter and sort results

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("IsActive eq true")
	.WithOrderBy("HireDate desc")
	.ToResult();

Console.WriteLine(result.Criteria); // "$filter=IsActive eq true&$orderby=HireDate desc"
Console.WriteLine($"Returned: {result.Items.Length}");
```

### 4. Apply paging and request the total count

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("Department.Name eq 'Engineering'")
	.WithOrderBy("Name")
	.WithPaging(skip: 20, top: 10, totalCount: true)
	.ToResult();

Console.WriteLine(result.Criteria); // "$skip=20&$top=10&$count=true&$filter=Department.Name eq 'Engineering'&$orderby=Name asc"
Console.WriteLine($"Returned: {result.Items.Length}");
Console.WriteLine($"Total: {result.TotalCount}");
```

Because `totalCount` is enabled, executing this query runs a second internal query to calculate `TotalCount`.

### 5. Parse and execute a complete OData-style query asynchronously

```csharp
var criteria = queryToolkit.Parser.Parse(
	"$filter=IsActive eq true&$orderby=HireDate desc&$skip=20&$top=10&$count=true");

var result = await employees
	.WithCriteria(criteria, queryToolkit)
	.ToResultAsync();

Console.WriteLine(result.Criteria); // "$skip=20&$top=10&$count=true&$filter=IsActive eq true&$orderby=HireDate desc"
Console.WriteLine($"Returned: {result.Items.Length}");
```

Because `$count=true` is specified, executing this query asynchronously also runs a second internal query to calculate `TotalCount`.

### 6. Minimal API with Entity Framework Core

In a Minimal API, bind the supported query parameters with `QueryCriteriaParams`, parse them through the injected `IQueryToolkit`, and execute the query with the EF Core executor:

```csharp
builder.Services.AddDbContext<CompanyDbContext>();
builder.Services.AddQueryServices(_ => _.WithEntityFrameworkCore());
builder.Services.AddScoped<EmployeesService>();

app.MapGet("/employees", async (
	[AsParameters] QueryCriteriaParams queryCriteriaParams,
	EmployeesService service) =>
{
    var queryCriteria = queryToolkit.Parser.Parse(queryCriteriaParams);

    var result = await ctx.Employees
        .Include(_ => _.Department) // use dto mapping/projection for real projects
        .WithCriteria("Get employees", queryCriteria, queryToolkit)
        .ToResultAsync();

    return Results.Ok(result);
});
```

For example, the following request applies a filter, ordering, paging, and a total count:

```http
GET /employees?$filter=IsActive%20eq%20true&$orderby=HireDate%20desc&$skip=0&$top=10&$count=true
```

### 7. ASP.NET Core MVC with Entity Framework Core

In an ASP.NET Core MVC controller, bind `QueryCriteriaParams` from the query string, parse the criteria through the injected `IQueryToolkit`, and execute the query with the EF Core executor:

```csharp
builder.Services.AddDbContext<CompanyDbContext>();
builder.Services.AddQueryServices(_ => _.WithEntityFrameworkCore());
builder.Services.AddControllers();

app.MapControllers();

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
```

For example, the following request can be handled by the controller:

```http
GET /employees?filter=IsActive%20eq%20true&orderby=HireDate%20desc&skip=0&top=10&count=true
```

The following examples assume an `IQueryable<Employee>` named `employees` and a configured `IQueryToolkit` named `queryToolkit`.

### OData-style filters reference

Bodde.Query supports the following comparison operators:

| Operator | Description | Example |
| --- | --- | --- |
| `eq` | Equal to | `Salary eq 50000` |
| `ne` | Not equal to | `Department.Name ne 'Sales'` |
| `gt` | Greater than | `Salary gt 50000` |
| `ge` | Greater than or equal to | `HireDate ge 2021-01-01` |
| `lt` | Less than | `Salary lt 50000` |
| `le` | Less than or equal to | `Salary le 50000` |
| `contains` | Contains a string | `Name contains 'John'` |
| `startswith` | Starts with a string | `Name startswith 'John'` |
| `endswith` | Ends with a string | `Name endswith 'son'` |
| `in` | Matches one of several values | `Department.Name in ('Sales', 'Engineering')` |

Multiple filters can be added with multiple `WithFilter` calls. They are combined using `and`:

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("IsActive eq true")
	.WithFilter("Salary gt 50000")
	.ToResult();

Console.WriteLine(result.Criteria); // "$filter=IsActive eq true and Salary gt 50000"
```

Use `and` to require all expressions to match:

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("IsActive eq true and Salary gt 50000")
	.ToResult();
```

Use `or` to match at least one expression:

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("Department.Name eq 'Sales' or Department.Name eq 'Engineering'")
	.ToResult();
```

Use `not` to negate a comparison or a parenthesized expression:

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("not (IsActive eq true)")
	.ToResult();
```

Logical operators can be combined in nested expressions by using parentheses:

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("(IsActive eq true and Salary gt 50000) or (Department.Name eq 'Sales' and HireDate ge 2021-01-01)")
	.ToResult();
```

### Limitations
The filter parser has limited OData support, and each comparison must be binary. Use parentheses or multiple `WithFilter` calls when composing complex criteria.

### Example projects with source code

The repository includes sample projects demonstrating different usage scenarios:

| Sample | Description |
| --- | --- |
| [Samples.ConsoleApp](https://github.com/bodde/Bodde.Query/tree/main/Samples/Samples.ConsoleApp) | Uses the toolkit directly without dependency injection. |
| [Samples.ConsoleApp.DI](https://github.com/bodde/Bodde.Query/tree/main/Samples/Samples.ConsoleApp.DI) | Uses the toolkit with dependency injection. |
| [Samples.ConsoleApp.EFCore](https://github.com/bodde/Bodde.Query/tree/main/Samples/Samples.ConsoleApp.EFCore) | Executes queries against Entity Framework Core. |
| [Samples.ConsoleApp.EFCore.DI](https://github.com/bodde/Bodde.Query/tree/main/Samples/Samples.ConsoleApp.EFCore.DI) | Combines Entity Framework Core with dependency injection. |
| [Samples.AspNetCore.MinimalApi](https://github.com/bodde/Bodde.Query/tree/main/Samples/Samples.AspNetCore.MinimalApi) | Exposes query criteria through an ASP.NET Core Minimal API. |
| [Samples.AspNetCore.Mvc](https://github.com/bodde/Bodde.Query/tree/main/Samples/Samples.AspNetCore.Mvc) | Exposes query criteria through an ASP.NET Mvc API. |

## Configuration

### Default service registration

`AddQueryServices` registers the default implementations of the query services, including `IQueryToolkit`, with a `Scoped` lifetime:

```csharp
builder.Services.AddQueryServices();
```

### Entity Framework Core

Use `WithEntityFrameworkCore` to replace the default LINQ executor with the Entity Framework Core executor:

```csharp
builder.Services.AddQueryServices(options => options
	.WithEntityFrameworkCore());
```

### Custom implementations

The service builder can replace individual components when custom parsing, formatting, expression building, handling, or execution is required:

```csharp
builder.Services.AddQueryServices(options => options
	.WithQueryCriteriaParser<CustomQueryCriteriaParser>()
	.WithQueryCriteriaFormatter<CustomQueryCriteriaFormatter>()
	.WithExpressionBuilder<CustomExpressionBuilder>()
	.WithQueryCriteriaHandler<CustomQueryCriteriaHandler>()
	.WithQueryExecutor<CustomQueryExecutor>());
```

The available customization methods are:

| Method | Interface to implement | Configures |
| --- | --- | --- |
| `WithQueryCriteriaParser<T>` | `IQueryCriteriaParser` | The service that parses query criteria. |
| `WithQueryCriteriaFormatter<T>` | `IQueryCriteriaFormatter` | The service that formats query criteria. |
| `WithExpressionBuilder<T>` | `IExpressionBuilder` | The service that builds LINQ expressions. |
| `WithQueryCriteriaHandler<T>` | `IQueryCriteriaHandler` | The service that applies criteria to an `IQueryable<T>`. |
| `WithQueryExecutor<T>` | `IQueryExecutor` | The service that materializes and counts query results. |

All custom implementations must implement the corresponding abstraction interface.

The default lifetime is `ServiceLifetime.Scoped` and can be changed when required:

```csharp
builder.Services.AddQueryServices(options => options
	.WithLifetime(ServiceLifetime.Singleton));
```

## Compatibility

The core Bodde.Query packages target `netstandard2.0`, while the Entity Framework
Core integration provides separate builds for `.NET 8` and `.NET 10`.

| Package | Compatibility |
| --- | --- |
| `Bodde.Query.Abstractions` | Targets `netstandard2.0`, has no external package dependencies, and can be referenced by compatible .NET and .NET Framework applications. |
| `Bodde.Query.Core` | Targets `netstandard2.0` and uses `Microsoft.Extensions.DependencyInjection.Abstractions` `8.0.0` for service registration. |
| `Bodde.Query.EntityFrameworkCore` | Targets `net8.0` and `net10.0`. The `net8.0` build uses Entity Framework Core `8.0.30`; the `net10.0` build uses Entity Framework Core `10.0.11`. |

The sample projects follow the runtime they demonstrate: `Samples.Common` and
`Samples.Common.EFCore` target both `net8.0` and `net10.0`, while the MVC sample
currently targets `net8.0`.

The library works with `IQueryable<T>` and can be used with LINQ providers that support the generated expression trees. When using Entity Framework Core, the final query must also be translatable by the configured database provider.

The OData support is intentionally partial and does not require `Microsoft.OData` or generated EDM models. Supported query constructs and operators are documented in the [Usage](#usage) section.

The query toolkit can be used with or without dependency injection and integrates with ASP.NET Core Minimal APIs and MVC controllers through `QueryCriteriaParams`.

## Versioning

Bodde.Query follows [Semantic Versioning](https://semver.org/) using the `MAJOR.MINOR.PATCH` format.

- `MAJOR` versions introduce breaking API or behavior changes.
- `MINOR` versions add backward-compatible features.
- `PATCH` versions include backward-compatible bug fixes and maintenance updates.

The Bodde.Query packages are versioned and released together. Keep the package versions aligned when installing multiple packages:

```text
Bodde.Query.Abstractions           1.0.0
Bodde.Query.Core                   1.0.0
Bodde.Query.EntityFrameworkCore    1.0.0
```

The package dependency chain is `Bodde.Query.EntityFrameworkCore` -> `Bodde.Query.Core` -> `Bodde.Query.Abstractions`. A major version of a package may require the corresponding major version of its dependencies.

Release notes and package versions are published with each release of the repository.

## Contributing

Contributions, bug reports, and feature requests are welcome.

### Development requirements

- .NET SDK 10.0 or later
- Git

### Build and test

Clone the repository, restore its dependencies, build the solution, and run the test suite:

```bash
git clone https://github.com/bodde/Bodde.Query.git
cd Bodde.Query
dotnet restore
dotnet build
dotnet test
```

The solution contains the core packages under `Bodde.Query.*`, automated tests under `Bodde.Query.Test`, and runnable examples under `Samples`.

### Pull requests

When submitting a pull request:

- Keep changes focused and consistent with the existing project structure.
- Add or update tests for behavioral changes.
- Update the README when changing public APIs, package behavior, or usage.
- Ensure `dotnet build` and `dotnet test` complete successfully.
- Describe the motivation and relevant design decisions in the pull request.

## License

Bodde.Query is released under the [MIT License](LICENSE).

Copyright (c) 2026 Tomaso Donini.