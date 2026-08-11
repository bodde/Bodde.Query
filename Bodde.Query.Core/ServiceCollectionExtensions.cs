using Bodde.Query.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bodde.Query.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueryServices(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        AddQueryServicesCore(services, lifetime);
        services.Add(new ServiceDescriptor(typeof(IQueryExecutor), typeof(DefaultQueryExecutor), lifetime));

        return services;
    }

    public static IServiceCollection AddQueryServices<TQueryExecutor>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TQueryExecutor : class, IQueryExecutor
    {

        AddQueryServicesCore(services, lifetime);
        services.AddScoped<IQueryExecutor, TQueryExecutor>();

        return services;
    }

    private static void AddQueryServicesCore(IServiceCollection services, ServiceLifetime lifetime)
    {
        services.Add(new ServiceDescriptor(typeof(IQueryCriteriaHandler), typeof(QueryCriteriaHandler), lifetime));
        services.Add(new ServiceDescriptor(typeof(IExpressionBuilder), typeof(ExpressionBuilder), lifetime));
        services.Add(new ServiceDescriptor(typeof(IQueryCriteriaFormatter), typeof(ODataFormatter), lifetime));
        services.Add(new ServiceDescriptor(typeof(IQueryCriteriaParser), typeof(ODataParser), lifetime));
    }
}
