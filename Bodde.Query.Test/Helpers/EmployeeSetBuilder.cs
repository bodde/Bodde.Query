using Bodde.Query.Test.Models;

namespace Bodde.Query.Test.Helpers;

public class EmployeeSetBuilder
{
    public static Employee[] Build()
    {
        var engineeringDepartment = new Department { Id = 1, DepartmentName = "Engineering" };
        var productDepartment = new Department { Id = 2, DepartmentName = "Product" };
        var designDepartment = new Department { Id = 3, DepartmentName = "Design" };

        var employees = new[]
        {
            new Employee { Id = 1,	FirstName = "Emma",		LastName = "Johnson",	Email = "emma.johnson@example.com",		    HireDate = new DateTime(2018, 5, 21),	Department = engineeringDepartment,	Role = Employee.RoleType.Developer, Salary = 90000m,    IsActive = true },
            new Employee { Id = 2,	FirstName = "Liam",		LastName = "Smith",		Email = "liam.smith@example.com",		    HireDate = new DateTime(2019, 3, 12),	Department = productDepartment,		Role = Employee.RoleType.Manager,   Salary = 110000m,   IsActive = true },
            new Employee { Id = 3,	FirstName = "Olivia",	LastName = "Brown",		Email = "olivia.brown@example.com",		    HireDate = new DateTime(2020, 7, 1),	Department = designDepartment,		Role = Employee.RoleType.Analyst,   Salary = 75000m,    IsActive = true },
            new Employee { Id = 4,	FirstName = "Noah",		LastName = "Davis",		Email = "noah.davis@example.com",		    HireDate = new DateTime(2017, 11, 5),	Department = engineeringDepartment,	Role = Employee.RoleType.Developer, Salary = 95000m,    IsActive = true },
            new Employee { Id = 5,	FirstName = "Ava",		LastName = "Miller",	Email = "ava.miller@example.com",		    HireDate = new DateTime(2021, 2, 18),	Department = engineeringDepartment,	Role = Employee.RoleType.Analyst,   Salary = 72000m,    IsActive = true },
            new Employee { Id = 6,	FirstName = "James",	LastName = "Wilson",	Email = "james.wilson@example.com",		    HireDate = new DateTime(2016, 9, 30),	Department = productDepartment,		Role = Employee.RoleType.Analyst,   Salary = 78000m,    IsActive = true },
            new Employee { Id = 7,	FirstName = "Sophia",	LastName = "Moore",		Email = "sophia.moore@example.com",		    HireDate = new DateTime(2018, 12, 10),	Department = designDepartment,		Role = Employee.RoleType.Manager,   Salary = 105000m,   IsActive = true },
            new Employee { Id = 8,	FirstName = "James",	LastName = "Taylor",	Email = "james.taylor@example.com",		    HireDate = new DateTime(2015, 6, 4),	Department = productDepartment,		Role = Employee.RoleType.Analyst,   Salary = 76000m,    IsActive = false },
            new Employee { Id = 9,	FirstName = "Isabella",	LastName = "Anderson",	Email = "isabella.anderson@example.com",	HireDate = new DateTime(2022, 1, 9),	Department = engineeringDepartment,	Role = Employee.RoleType.Analyst,   Salary = 70000m,    IsActive = true },
            new Employee { Id = 10,	FirstName = "Benjamin",	LastName = "Thomas",	Email = "ben.thomas@example.com",		    HireDate = new DateTime(2014, 4, 20),	Department = designDepartment,		Role = Employee.RoleType.Manager,   Salary = 100000m,   IsActive = false }
        };

        return employees;
    }
}
