using Bodde.Common.Extensions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Samples.Data;


var departments = DataSeeder.SeedDepartments().AsQueryable();
var filteredDepartments = departments;

var queryToolkit = new QueryToolkit();

var commands = new (string name, Func<IQueryable<Department>, IQueryable<Department>> apply)[]
{
    ("Reset", _ => departments),
    (
        "Add filter", _ =>
        {
            var input = Console.ReadLine() ?? string.Empty;
            if(input.IsEmpty())
                return _;

            var criteria = queryToolkit.Parser.ParseFilter(input);
            var queryCriteria = new QueryCriteria(Filter: criteria);

            return queryToolkit.Handler.ApplyCriteria(_, queryCriteria);
        }
    ),
    (
        "Add order by", _ =>
        {
           var input = Console.ReadLine() ?? string.Empty;
            if(input.IsEmpty())
                return _;

            var criteria = queryToolkit.Parser.ParseOrderBy(input);
            var queryCriteria = new QueryCriteria(OrderBy: criteria);

            return queryToolkit.Handler.ApplyCriteria(_, queryCriteria);
        }
    )
};

while (true)
{
    Console.WriteLine(filteredDepartments.ToDisplayTable(d => d.Id, d => d.Name));

    Console.WriteLine("Select command (or press Enter to exit)");
    for (int i = 0; i < commands.Length; i++)
    {
        var command = commands[i];
        Console.WriteLine($"{i}. {command.name}");
    }

    var input = Console.ReadLine() ?? string.Empty;
    if (input.IsEmpty())
        break;

    if (int.TryParse(input, out int commandIndex) && commandIndex >= 0 && commandIndex < commands.Length)
    {
        var command = commands[commandIndex];
        Console.WriteLine($"{command.name}: ");
        try
        {
            filteredDepartments = command.apply(filteredDepartments);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
    }
}
