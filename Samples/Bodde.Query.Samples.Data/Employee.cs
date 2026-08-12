namespace Bodde.Query.Samples.Data;

public class Employee
{
    public int Id { get; set; }

    public required string Name { get; set; }
    
    public int DepartmentId { get; set; }
    
    public Department? Department { get; set; }

    public DateTime HireDate { get; set; }

    public int Salary { get; set; }
}
