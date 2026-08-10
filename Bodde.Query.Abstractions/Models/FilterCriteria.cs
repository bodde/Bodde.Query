namespace Bodde.Query.Abstractions.Models;

public record FilterCriteria(FilterCriteria.FilterExpression Expression)
{
    public abstract record FilterExpression;

    public record ComparisonExpression(string PropertyPath, ComparisonOperator Operator, object? Value) : FilterExpression;

    public record NotExpression(FilterExpression Expression) : FilterExpression;

    public record LogicalExpression(LogicalOperator Operator, FilterExpression First, FilterExpression Second, params FilterExpression[] Others) : FilterExpression
    {
        public FilterExpression[] AllExpressions 
        { 
            get
            {
                FilterExpression[] expressions =
                [
                    First,
                    Second,
                    ..Others
                ];

                return expressions;
            }
        }
    }

    public enum ComparisonOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith,
        In,
    }

    public enum LogicalOperator
    {
        And,
        Or
    }
}

