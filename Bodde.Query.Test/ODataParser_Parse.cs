using System;
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
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void EmptyString_NullCriteria(string? input)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        var result = sut.Parse(input);
#pragma warning restore CS8604 // Possible null reference argument. 

        Assert.Null(result);
    }

    [Theory]
    [InlineData("$skip=10&$top=20&$count=true&$filter=Name eq 'John'&$orderby=Age desc", true, true, true)]
    [InlineData("$filter=Name eq 'John'&$skip=10&$top=20&$count=true&$orderby=Age desc", true, true, true)]
    [InlineData("$orderby=Age desc&$filter=Name eq 'John'&$skip=10&$top=20&$count=true", true, true, true)]
    [InlineData("$skip=10&$top=20", true, false, false)]
    [InlineData("$filter=Name eq 'John'", false, true, false)]
    [InlineData("$orderby=Age desc", false, false, true)]
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
