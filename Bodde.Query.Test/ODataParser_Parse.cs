using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class ODataParser_Parse
{
    private readonly ODataParser sut;

    public ODataParser_Parse()
    {
        sut = new ODataParser();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void EmptyString_EmptyCriteria(string input)
    {
        var result = sut.Parse(input);

        Assert.NotNull(result);
        Assert.Null(result.Filter);
        Assert.Null(result.OrderBy);
        Assert.NotNull(result.Paging);
    }

    [Theory]
    [InlineData(null, null, null, null, null)]
    [InlineData("Name eq 'John'", "Age desc", 10, 20, true)]
    public void WithParameters_ExpectedCriteria(string? filter, string? orderBy, int? skip, int? top, bool? totalCount)
    {
        var result = sut.Parse(filter, orderBy, skip, top, totalCount);

        Assert.NotNull(result);

        if(filter == null)
        {
            Assert.Null(result.Filter);
        }
        else
        {
            Assert.NotNull(result.Filter);
        }
        
        if(orderBy == null)
        {
            Assert.Null(result.OrderBy);
        }
        else
        {
            Assert.NotNull(result.OrderBy);
        }

        Assert.NotNull(result.Paging);
        
        if(skip == null)
        {
            Assert.Null(result.Paging.Skip);
        }
        else
        {
            Assert.NotNull(result.Paging.Skip);
        }
               
        if(top == null)
        {
            Assert.Null(result.Paging.Top);
        }
        else
        {
            Assert.NotNull(result.Paging.Top);
        }
                       
        if(totalCount == null)
        {
            Assert.Null(result.Paging.TotalCount);
        }
        else
        {
            Assert.NotNull(result.Paging.TotalCount);
        }
    }

    [Theory]
    [InlineData("$skip=10&$top=20&$count=true&$filter=Name eq 'John'&$orderby=Age desc", true, true, true)]
    [InlineData("$filter=Name eq 'John'&$skip=10&$top=20&$count=true&$orderby=Age desc", true, true, true)]
    [InlineData("$orderby=Age desc&$filter=Name eq 'John'&$skip=10&$top=20&$count=true", true, true, true)]
    [InlineData("$skip=10&$top=20", true, false, false)]
    [InlineData("$filter=Name eq 'John'", true, true, false)]
    [InlineData("$orderby=Age desc", true, false, true)]
    public void NotEmpty_Criteria(string input, bool hasPaging, bool hasFilter, bool hasOrderBy)
    {
        var result = sut.Parse(input);

        if (hasPaging)
        {
            Assert.NotNull(result?.Paging);
        }
        else
        {
            Assert.Null(result?.Paging);
        }

        if (hasFilter)
        {
            Assert.NotNull(result?.Filter);
        }
        else
        {
            Assert.Null(result?.Filter);
        }

        if (hasOrderBy)
        {
            Assert.NotNull(result?.OrderBy);
        }
        else
        {
            Assert.Null(result?.OrderBy);
        }
    }
}
