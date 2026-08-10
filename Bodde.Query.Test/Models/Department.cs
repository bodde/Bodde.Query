namespace Bodde.Query.Test.Models;

public class Department : IIdentifiable<int>
{
    public int Id { get; set; }

    public required string DepartmentName { get; set; }
}
