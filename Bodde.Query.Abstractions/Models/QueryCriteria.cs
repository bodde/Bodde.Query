namespace Bodde.Query.Abstractions.Models;

public record QueryCriteria(FilterCriteria? Filter = null, OrderByCriteria? OrderBy = null, PagingCriteria? Paging = null);
