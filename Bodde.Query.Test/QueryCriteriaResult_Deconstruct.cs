using Bodde.Query.Abstractions.Models;

namespace Bodde.Query.Test;

public class QueryCriteriaResult_Deconstruct
{
	[Fact]
	public void Deconstruct_Yields_Items_And_TotalCount()
	{
		var r = new QueryCriteriaResult<int>([7, 8], 2);
		var (items, total) = r;
		Assert.Equal([7, 8], items);
		Assert.Equal(2, total);
	}

    [Fact]
    public void Deconstruct_Yields_Items_And_Null_TotalCount()
    {
        var r = new QueryCriteriaResult<int>([7, 8]);
        var (items, total) = r;
        Assert.Equal([7, 8], items);
        Assert.Null(total);
    }
}