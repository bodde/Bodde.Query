using Bodde.Query.Test.Models;
using static Bodde.Query.Abstractions.Models.FilterCriteria;
using static Bodde.Query.Abstractions.Models.OrderByCriteria;

namespace Bodde.Query.Test.Helpers;

internal static class QueryCriteriaItemBuilder
{
    public static readonly ComparisonExpression SalaryGreaterThan80000 = new(nameof(Employee.Salary), ComparisonOperator.GreaterThan, 80000);

    public static readonly OrderByItem OrderByLastName = new(nameof(Employee.LastName), SortDirection.Ascending);

    internal static ComparisonExpression RoleManager = new(nameof(Employee.Role), ComparisonOperator.LessThan, Employee.RoleType.Manager);
}
