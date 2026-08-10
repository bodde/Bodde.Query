namespace Bodde.Query.Abstractions.Models;

public record QueryCriteria(FilterCriteria? Filter = null, OrderByCriteria? OrderBy = null, PagingCriteria? Paging = null)
{
    public void Deconstruct(
        out FilterCriteria? filter,
        out OrderByCriteria? orderBy,
        out PagingCriteria? paging)
    {
        filter = Filter;
        orderBy = OrderBy;
        paging = Paging;
    }
}
