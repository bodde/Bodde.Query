using static Bodde.Query.Core.ServiceCollectionExtensions;

namespace Bodde.Query.EntityFrameworkCore;

/// <summary>
/// Provides dependency-injection configuration extensions for Entity Framework Core query services.
/// </summary>
public static class QueryServicesBuilderExtensions
{
    /// <summary>
    /// Configures the query services to use <see cref="EntityFrameworkCoreQueryExecutor"/>.
    /// </summary>
    /// <param name="builder">The query services builder to configure.</param>
    /// <returns>The configured query services builder.</returns>
    public static IQueryServicesBuilder WithEntityFrameworkCore(this IQueryServicesBuilder builder)
    {
        return builder.WithQueryExecutor<EntityFrameworkCoreQueryExecutor>();
    }
}