# Bodde.Query

Bodde.Query is a lightweight set of NuGet packages for building, parsing, formatting, and executing query criteria.

It provides reusable support for filtering, sorting, and paging `IQueryable<T>` data sources.

It supports a partial, limited subset of OData query constructs, including `$filter`, `$orderby`, `$top`, `$skip`, and `$count`. These constructs can describe queries without requiring a dependency on `Microsoft.OData` or the generation of EDM models.

The toolkit is extensible and includes integrations for Entity Framework Core.

## Packages

### Bodde.Query.Abstractions

Contains the core models and service contracts used to define, parse, format, and execute query criteria.

### Bodde.Query.Core

Provides the main query criteria implementation, including OData-inspired parsing and formatting, LINQ expression building, and dependency injection support.

### Bodde.Query.EntityFrameworkCore

Adds Entity Framework Core integration for executing query criteria against EF Core data sources.

## Installation

Install the package that matches your application requirements:

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

For Entity Framework Core support, install the additional integration package:

```bash
dotnet add package Bodde.Query.EntityFrameworkCore
```

When using Entity Framework Core, configure the EF Core query executor:

```csharp
services.AddQueryServices(builder => builder.WithEntityFrameworkCore());
```

## Usage

### Getting the query toolkit

With dependency injection, register the query services and inject `IQueryToolkit` into the class that executes the query:

```csharp
services.AddQueryServices();

public class EmployeeService(IQueryToolkit queryToolkit)
{
}
```

Without dependency injection, create the default implementation directly:

```csharp
IQueryToolkit queryToolkit = QueryToolkit.Default();
```

The following examples assume an `IQueryable<Employee>` named `employees` and a configured `IQueryToolkit` named `queryToolkit`.

### 1. Execute a query without criteria

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.ToResult();
```

`WithCriteria` returns a `QueryWithCriteria<T>` instance, which wraps the source query and the selected toolkit. This type enables the other query extension methods, such as `WithName`, `WithFilter`, `WithOrderBy`, and `WithPaging`, to be chained before execution.

### 1. Filter results

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("IsActive eq true")
	.ToResult();
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
```

### 3. Filter and sort results

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("IsActive eq true")
	.WithOrderBy("HireDate desc")
	.ToResult();
```

### 4. Apply paging and request the total count

```csharp
var result = employees
	.WithCriteria(queryToolkit)
	.WithFilter("Department.Name eq 'Engineering'")
	.WithOrderBy("Name")
	.WithPaging(skip: 20, top: 10, totalCount: true)
	.ToResult();

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
```

Because `$count=true` is specified, executing this query asynchronously also runs a second internal query to calculate `TotalCount`.

### Supported OData-style filters

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

The filter parser has limited OData support, and each comparison must be binary. Use parentheses and multiple `WithFilter` calls when composing complex criteria.


## Configuration


## Features


## Examples


## Compatibility


## Versioning


## Contributing


## License