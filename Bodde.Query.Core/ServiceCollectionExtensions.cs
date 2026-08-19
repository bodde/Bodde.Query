using Bodde.Query.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bodde.Query.Core;

/// <summary>
/// Provides dependency-injection registration extensions for query services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the default query services and optionally configures their implementations and lifetime.
    /// </summary>
    /// <typeparam name="TServiceCollection">The service collection type.</typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="builder">An optional callback used to configure query service registrations.</param>
    /// <returns>The configured service collection.</returns>
    public static TServiceCollection AddQueryServices<TServiceCollection>(this TServiceCollection services, Action<IQueryServicesBuilder>? builder = null)
        where TServiceCollection : IServiceCollection
    {        
        var queryServicesBuilder = new QueryServicesBuilder();
        builder?.Invoke(queryServicesBuilder);

        queryServicesBuilder.Build().ForEach(services.Add);

        return services;
    }

    /// <summary>
    /// Configures the implementations and lifetime of the registered query services.
    /// </summary>
    public interface IQueryServicesBuilder
    {
        /// <summary>Sets the query criteria handler implementation.</summary>
        /// <typeparam name="TQueryCriteriaHandler">The handler implementation type.</typeparam>
        /// <returns>This builder.</returns>
        public IQueryServicesBuilder WithQueryCriteriaHandler<TQueryCriteriaHandler>() where TQueryCriteriaHandler : class, IQueryCriteriaHandler;
        /// <summary>Sets the expression builder implementation.</summary>
        /// <typeparam name="TExpressionBuilder">The expression builder implementation type.</typeparam>
        /// <returns>This builder.</returns>
        public IQueryServicesBuilder WithExpressionBuilder<TExpressionBuilder>() where TExpressionBuilder : class, IExpressionBuilder;
        /// <summary>Sets the query criteria formatter implementation.</summary>
        /// <typeparam name="TQueryCriteriaFormatter">The formatter implementation type.</typeparam>
        /// <returns>This builder.</returns>
        public IQueryServicesBuilder WithQueryCriteriaFormatter<TQueryCriteriaFormatter>() where TQueryCriteriaFormatter : class, IQueryCriteriaFormatter;
        /// <summary>Sets the query criteria parser implementation.</summary>
        /// <typeparam name="TQueryCriteriaParser">The parser implementation type.</typeparam>
        /// <returns>This builder.</returns>
        public IQueryServicesBuilder WithQueryCriteriaParser<TQueryCriteriaParser>() where TQueryCriteriaParser : class, IQueryCriteriaParser;
        /// <summary>Sets the query executor implementation.</summary>
        /// <typeparam name="TQueryExecutor">The executor implementation type.</typeparam>
        /// <returns>This builder.</returns>
        public IQueryServicesBuilder WithQueryExecutor<TQueryExecutor>() where TQueryExecutor : class, IQueryExecutor;
        /// <summary>Sets the lifetime used for registered query services.</summary>
        /// <param name="lifetime">The service lifetime.</param>
        /// <returns>This builder.</returns>
        public IQueryServicesBuilder WithLifetime(ServiceLifetime lifetime);
    }

    private class QueryServicesBuilder : IQueryServicesBuilder
    {
        ServiceLifetime lifetime = ServiceLifetime.Scoped;

        Type queryCriteriaHandlerType = typeof(QueryCriteriaHandler);
        Type expressionBuilderType = typeof(ExpressionBuilder);
        Type queryCriteriaFormatterType = typeof(ODataFormatter);
        Type queryCriteriaParserType = typeof(ODataParser);
        Type queryExecutorType = typeof(LinqQueryExecutor);

        public List<ServiceDescriptor> Build()
        {
            return
            [
                new(typeof(IQueryCriteriaHandler), queryCriteriaHandlerType, lifetime),
                new(typeof(IExpressionBuilder), expressionBuilderType, lifetime),
                new(typeof(IQueryCriteriaFormatter), queryCriteriaFormatterType, lifetime),
                new(typeof(IQueryCriteriaParser), queryCriteriaParserType, lifetime),
                new(typeof(IQueryExecutor), queryExecutorType, lifetime),
                new(typeof(IQueryToolkit), typeof(QueryToolkit), lifetime)
            ];
        }

        public IQueryServicesBuilder WithLifetime(ServiceLifetime lifetime)
        {
            this.lifetime = lifetime;
            return this;
        }

        IQueryServicesBuilder IQueryServicesBuilder.WithExpressionBuilder<TExpressionBuilder>()
        {
            expressionBuilderType = typeof(TExpressionBuilder);
            return this;
        }

        IQueryServicesBuilder IQueryServicesBuilder.WithQueryCriteriaFormatter<TQueryCriteriaFormatter>()
        {
            queryCriteriaFormatterType = typeof(TQueryCriteriaFormatter);
            return this;
        }

        IQueryServicesBuilder IQueryServicesBuilder.WithQueryCriteriaHandler<TQueryCriteriaHandler>()
        {
            queryCriteriaHandlerType = typeof(TQueryCriteriaHandler);
            return this;
        }

        IQueryServicesBuilder IQueryServicesBuilder.WithQueryCriteriaParser<TQueryCriteriaParser>()
        {
            queryCriteriaParserType = typeof(TQueryCriteriaParser);
            return this;
        }

        IQueryServicesBuilder IQueryServicesBuilder.WithQueryExecutor<TQueryExecutor>()
        {
            queryExecutorType = typeof(TQueryExecutor);
            return this;
        }
    }
}
