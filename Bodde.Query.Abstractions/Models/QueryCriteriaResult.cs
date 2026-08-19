namespace Bodde.Query.Abstractions.Models;

/// <summary>
/// Represents the result of executing a query with criteria.
/// </summary>
/// <typeparam name="T">The type of the returned items.</typeparam>
/// <param name="Name">The name of the query.</param>
/// <param name="Criteria">The formatted criteria used by the query.</param>
/// <param name="Items">The items returned by the query.</param>
/// <param name="TotalCount">The total number of matching items, if requested.</param>
public record QueryCriteriaResult<T>(string Name, string Criteria, T[] Items, int? TotalCount = null);
