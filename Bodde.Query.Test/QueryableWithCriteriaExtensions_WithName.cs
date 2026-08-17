using Bodde.Query.Abstractions.Extensions;
using Bodde.Query.Abstractions.Services;
using Bodde.Query.Test.Helpers;
using Moq;

namespace Bodde.Query.Test;

public class QueryableWithCriteriaExtensions_WithName
{    
    [Fact]
    public void NullName_Throw()
    {
        var toolkit = new Mock<IQueryToolkit>();

        var sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.Throws<ArgumentNullException>(() => sut.WithName(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("test")]
    public void ValidName(string name)
    {
        var toolkit = new Mock<IQueryToolkit>();

        var sut = EmployeeSetBuilder.Build().AsQueryable().WithCriteria(toolkit.Object);

        var actual = sut.WithName(name);

        Assert.NotNull(actual);
        Assert.Equal(name, actual.Name);
    }
}