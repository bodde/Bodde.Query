using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Models;
using Bodde.Query.Test.Extensions;
using COp = Bodde.Query.Abstractions.Models.FilterCriteria.ComparisonOperator;
using LOp = Bodde.Query.Abstractions.Models.FilterCriteria.LogicalOperator;

namespace Bodde.Query.Test;

public class QueryCriteriaHandler_ApplyCriteria
{
    private readonly IQueryable<Employee> data;
    private readonly QueryCriteriaHandler sut;

    public QueryCriteriaHandler_ApplyCriteria()
    {
        data = EmployeeSetBuilder.Build().AsQueryable();
        sut = new QueryCriteriaHandler(new ExpressionBuilder());
    }

    [Fact]
    public void NoPaging()
    {
        var expected = data.ToArray();
        var criteria = new QueryCriteria();

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }

    [Fact]
    public void Top5()
    {
        var top = 5;
        var expected = data.Take(top).ToArray();

        var criteria = new QueryCriteria(
            Paging: new (Top: top)
            );

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }

    [Fact]
    public void Skip2()
    {
        var skip = 2;
        var expected = data.Skip(skip).ToArray();

        var criteria = new QueryCriteria(
            Paging: new (Skip: skip)
            );

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }

    [Fact]
    public void Skip2_Top3()
    {     
        var skip = 2;
        var top = 3;
        var expected = data.Skip(skip).Take(top).ToArray();
        
        var criteria = new QueryCriteria(
            Paging: new (Skip: skip, Top: top)
        );    

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }


    [Fact]
    public void FilterBy_Id_Equals_4()
    {        
        var id = 4;
        var expected = data.Where(e => e.Id == id).ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(new FilterCriteria.ComparisonExpression(
                PropertyPath: nameof(Employee.Id),
                Operator: COp.Equals,
                Value: id
                )
            ));      
        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv()); 
    }

    [Fact]
    public void FilterBy_FirstName_Equals_James()
    {        
        var name = "James";
        var expected = data.Where(e => e.FirstName == name).ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(new FilterCriteria.ComparisonExpression(
                PropertyPath: nameof(Employee.FirstName),
                Operator: COp.Equals,
                Value: name
                )
            ));      
        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv()); 
    }


    [Fact]
    public void FilterBy_HireDate_LessThan_2016()
    {
        var date = new DateTime(2016, 1, 1);
        var expected = data.Where(e => e.HireDate < date).ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(new FilterCriteria.ComparisonExpression(
                PropertyPath: nameof(Employee.HireDate),
                Operator: COp.LessThan,
                Value: date
                )
            ));      
        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv()); 
    }

    [Fact]
    public void FilterBy_Salary_GreaterThan_80000()
    {
        var salary = 80000m;
        var expected = data.Where(e => e.Salary > salary).ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(new FilterCriteria.ComparisonExpression(
                PropertyPath: nameof(Employee.Salary),
                Operator: COp.GreaterThan,
                Value: salary
                )
            ));      
        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv()); 
    }

    [Fact]
    public void FilterBy_Role_Developer()
    {
        var role = Employee.RoleType.Developer;
        var expected = data.Where(e => e.Role == role).ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(new FilterCriteria.ComparisonExpression(
                PropertyPath: nameof(Employee.Role),
                Operator: COp.Equals,
                Value: Employee.RoleType.Developer
                )
            ));      
        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv()); 
    }

    [Fact]
    public void FilterBy_IsActive_True()
    {
        var isActive = true;
        var expected = data.Where(e => e.IsActive == isActive).ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(new FilterCriteria.ComparisonExpression(
                PropertyPath: nameof(Employee.IsActive),
                Operator: COp.Equals,
                Value: isActive
                )
            ));      
        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv()); 
    }

    [Fact]
    public void FilterBy_FirstName_Contains_m()
    {
        var substring = "m";
        var expected = data.Where(e => e.FirstName.Contains(substring)).ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(new FilterCriteria.ComparisonExpression(
                PropertyPath: nameof(Employee.FirstName),
                Operator: COp.Contains,
                Value: substring
                )
            ));      
        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv()); 
    }

    [Fact]
    public void FilterBy_LastName_StartsWith_M()
    {
        var substring = "M";
        var expected = data.Where(e => e.LastName.StartsWith(substring)).ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(new FilterCriteria.ComparisonExpression(
                PropertyPath: nameof(Employee.LastName),
                Operator: COp.StartsWith,
                Value: substring
                )
            ));      
        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }

    [Fact]
    public void FilterBy_Email_EndsWith_r_amp_example_dot_com()
    {
        var substring = "r@example.com";
        var expected = data.Where(e => e.Email.EndsWith(substring)).ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(new FilterCriteria.ComparisonExpression(
                PropertyPath: nameof(Employee.Email),
                Operator: COp.EndsWith,
                Value: substring
                )
            ));
        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }


    [Fact]
    public void FilterBy_DepartmentName_Equals_Engineering()
    {
        var departmentName = "Engineering";
        var expected = data
            .Where(e => e.Department != null && e.Department.DepartmentName == departmentName)
            .ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(new FilterCriteria.ComparisonExpression(
                PropertyPath: "Department.DepartmentName",
                Operator: COp.Equals,
                Value: departmentName
                )
            ));      
        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }

    [Fact]
    public void FilterBy_IsActive_True_And_Salary_GreaterThan_60000()
    {
        var isActive = true;
        var salary = 60000m;
        var expected = data
            .Where(e => e.IsActive == isActive && e.Salary > salary)
            .ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(
                new FilterCriteria.LogicalExpression(
                    Operator: LOp.And,
                    new FilterCriteria.ComparisonExpression(nameof(Employee.IsActive), COp.Equals, isActive),
                    new FilterCriteria.ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, salary)
                )
            ));

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv()); 
    }

    [Fact]
    public void FilterBy_IsActive_True_And_Salary_GreaterThan_80000_And_HireDate_LessThan_2020()
    {
        var isActive = true;
        var salary = 60000m;
        var hireDate = new DateTime(2020, 1, 1);
        var expected = data
            .Where(e => e.IsActive == isActive && e.Salary > salary && e.HireDate < hireDate)
            .ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(
                new FilterCriteria.LogicalExpression(
                    Operator: LOp.And,
                    new FilterCriteria.ComparisonExpression(nameof(Employee.IsActive), COp.Equals, isActive),
                    new FilterCriteria.ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, salary),
                    new FilterCriteria.ComparisonExpression(nameof(Employee.HireDate), COp.LessThan, hireDate)
                )
            ));

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv()); 
    }

    [Fact]
    public void FilterBy_Salary_GreaterThan_100000_Or_Salary_LessThan_80000()
    {
        var salary1 = 100000m;
        var salary2 = 80000m;
        var expected = data
            .Where(e => e.Salary > salary1 || e.Salary < salary2)
            .ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(
                new FilterCriteria.LogicalExpression(
                    Operator: LOp.Or,
                    new FilterCriteria.ComparisonExpression(nameof(Employee.Salary), COp.GreaterThan, salary1),
                    new FilterCriteria.ComparisonExpression(nameof(Employee.Salary), COp.LessThan, salary2)
                )
            ));

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv()); 
    }

    [Fact]
    public void FilterBy_IsActive_False_Or_HireDate_Between_2015_2018()
    {
        var isActive = false;
        var startDate = new DateTime(2015, 1, 1);
        var endDate = new DateTime(2018, 12, 31);
        var expected = data
            .Where(e => e.IsActive == isActive || (e.HireDate >= startDate && e.HireDate <= endDate))
            .ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(
                new FilterCriteria.LogicalExpression(
                    Operator: LOp.Or,
                    new FilterCriteria.ComparisonExpression(nameof(Employee.IsActive), COp.Equals, isActive),
                    new FilterCriteria.LogicalExpression(
                        Operator: LOp.And,
                        new FilterCriteria.ComparisonExpression(nameof(Employee.HireDate), COp.GreaterThanOrEqual, startDate),
                        new FilterCriteria.ComparisonExpression(nameof(Employee.HireDate), COp.LessThanOrEqual, endDate)
                    )
                )
            ));

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }

    [Fact]
    public void FilterBy_Role_In_Engineering_Analyst()
    {
        var roles = new[] { Employee.RoleType.Developer, Employee.RoleType.Analyst };
        var expected = data
            .Where(e => roles.Contains(e.Role))
            .ToArray();

        var criteria = new QueryCriteria(
            Filter: new FilterCriteria(
                new FilterCriteria.ComparisonExpression(
                    PropertyPath: nameof(Employee.Role),
                    Operator: COp.In,
                    Value: roles
                )
            ));

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }

    [Fact]
    public void OrderBy_None()
    {
        var expected = data.ToArray();

        var criteria = new QueryCriteria(
            OrderBy: null);

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }

    [Fact]
    public void OrderBy_FirstName_Ascending()
    {
        var expected = data.OrderBy(e => e.FirstName).ToArray();

        var criteria = new QueryCriteria(
            OrderBy: new OrderByCriteria(
                new OrderByCriteria.OrderByItem(
                    PropertyPath: nameof(Employee.FirstName),
                    Direction: OrderByCriteria.SortDirection.Ascending
                )
            ));

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }

    [Fact]
    public void OrderBy_Department_Name_Ascending_Then_By_Salary_Descending()
    {
        var expected = data
            .OrderBy(e => e.Department!.DepartmentName)
            .ThenByDescending(e => e.Salary).ToArray();

        var criteria = new QueryCriteria(
            OrderBy: new OrderByCriteria(
                new OrderByCriteria.OrderByItem(
                    PropertyPath: "Department.DepartmentName",
                    Direction: OrderByCriteria.SortDirection.Ascending
                ),
                new OrderByCriteria.OrderByItem(
                    PropertyPath: nameof(Employee.Salary),
                    Direction: OrderByCriteria.SortDirection.Descending
                )
            ));

        var result = sut.ApplyCriteria(data, criteria).ToArray();

        Assert.Equal(expected.GetIdsCsv(), result.GetIdsCsv());
    }
}

