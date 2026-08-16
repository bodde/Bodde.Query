using static Bodde.Query.Core.ServiceCollectionExtensions;

namespace Bodde.Query.EntityFrameworkCore;

public static class QueryServicesBuilderExtensions
{
    public static IQueryServicesBuilder WithEntityFrameworkCore(this IQueryServicesBuilder builder)
    {
        return builder.WithQueryExecutor<EntityFrameworkCoreQueryExecutor>();
    }
}