using Bodde.Query.Abstractions.Services;
using Bodde.Query.Core;
using Moq;

namespace Bodde.Query.Test;

public class QueryCriteriaDefaultToolkit_Props
{
    [Fact]
    public void Props_AreNotNull()
    {
        var sut = new QueryCriteriaDefaultToolkit(new Mock<IQueryExecutor>().Object);

        Assert.NotNull(sut.Formatter);
        Assert.NotNull(sut.Parser);
        Assert.NotNull(sut.Handler);
        Assert.NotNull(sut.Executor);
        Assert.NotNull(sut.ExpressionBuilder);
    }
}
