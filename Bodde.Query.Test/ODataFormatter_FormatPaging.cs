using System;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class ODataFormatter_FormatPaging
{
    private readonly ODataFormatter sut;

    public ODataFormatter_FormatPaging()
    {
        sut = new ODataFormatter();
    }

    [Fact]
    public void PagingCriteria_SkipOnly()
    {
        var paging = new PagingCriteria(Skip: 5, Top: null, TotalCount: null);

        var result = sut.FormatPaging(paging);
        
        Assert.Equal("$skip=5", result);
    }

    [Fact]
    public void PagingCriteria_TopOnly()
    {
        var paging = new PagingCriteria(Skip: null, Top: 20, TotalCount: null);
        var result = sut.FormatPaging(paging);
        Assert.Equal("$top=20", result);
    }

    [Fact]
    public void PagingCriteria_TotalCountOnly()
    {
        var paging = new PagingCriteria(Skip: null, Top: null, TotalCount: true);
        var result = sut.FormatPaging(paging);
        Assert.Equal("$count=true", result);
    }

    [Fact]
    public void PagingCriteria_AllSet()
    {
        var paging = new PagingCriteria(Skip: 10, Top: 50, TotalCount: true);
        var result = sut.FormatPaging(paging);
        Assert.Equal("$skip=10&$top=50&$count=true", result);
    }
}
