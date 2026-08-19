using Bodde.Query.Core;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Test.Helpers;
using Bodde.Query.Test.Mocked;
using Bodde.Query.Test.Models;

namespace Bodde.Query.Test;

public class QueryWithCriteriaExtensions_WithName
{        
    private readonly QueryToolkitMock toolkit;
    private readonly QueryWithCriteria<Employee> sut;

    public QueryWithCriteriaExtensions_WithName()
    {
        toolkit = new QueryToolkitMock();
        sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);
    }

    [Fact]
    public void NullName_Throw()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => sut.WithName(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("test")]
    public void ValidName(string name)
    {
        var actual = sut.WithName(name);

        Assert.NotNull(actual);
        Assert.Equal(name, actual.Name);
    }
}