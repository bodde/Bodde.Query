using Bodde.Query.Core;
using Samples.Common;

var departments = DataSeeder.SeedDepartments();
var employees = DataSeeder.SeedEmployees(departments);

var queryToolkit = QueryToolkit.Default();
var testExecutor = new QueryTester(employees, _ => Console.WriteLine(_), queryToolkit);

testExecutor.Execute();

