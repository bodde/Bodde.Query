namespace Bodde.Query.Samples.Data;

public static class DataSeeder
{
    public static IEnumerable<Department> SeedDepartments()
    {
        return [
          new Department { Name = "Human Resources" },
          new Department { Name = "Engineering" },
          new Department { Name = "Marketing" }
        ];
    }

    public static IEnumerable<Employee> SeedEmployees(IEnumerable<Department> departments)
    {
        var departmentsArray = departments.ToArray();

        string[] firstNames = ["John", "Jane", "Bob", "Alice", "Charlie", "David"];
        string[] lastNames = ["Doe", "Smith", "Johnson", "Williams", "Brown", "Jones"];

        var departmentIndex = 0;
        var id = 1;
        var hireDate = new DateTime(2020, 1, 1);

        foreach(var firstName in firstNames)
        {
            foreach(var lastName in lastNames)
            {
                var department = departmentsArray[departmentIndex];
                departmentIndex = (departmentIndex + 1) % departmentsArray.Length;
                hireDate = hireDate.AddMonths(1);

                yield return new Employee
                {
                    Id = id++,
                    Name = $"{firstName} {lastName}",
                    DepartmentId = department.Id,
                    Department = department,
                    HireDate = hireDate,
                    Salary = 50000 + (id * 1000)
                };
            }
        }
    }

}
