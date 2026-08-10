using System;
using System.Runtime.InteropServices;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class ODataParser_ParsePaging
{
    private readonly ODataParser sut;

    public ODataParser_ParsePaging()
    {
        sut = new ODataParser();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Empty_NullCriteria(string? input)
    {
#pragma warning disable CS8604 // Possible null reference argument.
        var result = sut.ParsePaging(input);
#pragma warning restore CS8604 // Possible null reference argument.

        Assert.Null(result);
    }

    [Theory]
    [InlineData("$skip=10&$top=20&$count=true", 10, 20, true)]
    [InlineData("$top=20&$count=true&$skip=10", 10, 20, true)]
    [InlineData("$count=true&$skip=10&$top=20", 10, 20, true)]
    [InlineData("$skip=5&$top=15&$count=false", 5, 15, false)]
    [InlineData("$top=15&$skip=5&$count=false", 5, 15, false)]
    [InlineData("$count=false&$top=15&$skip=5", 5, 15, false)]
    [InlineData("$skip=10", 10, null, null)]
    [InlineData("$top=25", null, 25, null)]
    [InlineData("$count=true", null, null, true)]
    [InlineData("$count=false", null, null, false)]
    [InlineData("$top=15&$skip=5", 5, 15, null)]
    [InlineData("$skip=5&$count=true", 5, null, true)]
    [InlineData("$top=20&$count=false", null, 20, false)]
    public void NotEmpty_PagingCriteria(string input, int? expectedSkip, int? expectedTop, bool? expectedCount)
    {
        var result = sut.ParsePaging(input);

        var expected = new PagingCriteria(Skip: expectedSkip, Top: expectedTop, TotalCount: expectedCount);

        Assert.Equal(expected, result);
    }

}
