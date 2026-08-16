using Bodde.Query.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bodde.Query.Core;

public static class ServiceCollectionExtensions
{
    public static TServiceCollection AddQueryServices<TServiceCollection>(this TServiceCollection services, Action<IQueryServicesBuilder>? builder = null)
        where TServiceCollection : IServiceCollection
    {        
        var queryServicesBuilder = new QueryServicesBuilder();
        builder?.Invoke(queryServicesBuilder);

        queryServicesBuilder.Build().ForEach(sd => services.Add(sd));

        return services;
    }

    public interface IQueryServicesBuilder
    {
        public IQueryServicesBuilder WithQueryCriteriaHandler<TQueryCriteriaHandler>() where TQueryCriteriaHandler : class, IQueryCriteriaHandler;
        public IQueryServicesBuilder WithExpressionBuilder<TExpressionBuilder>() where TExpressionBuilder : class, IExpressionBuilder;
        public IQueryServicesBuilder WithQueryCriteriaFormatter<TQueryCriteriaFormatter>() where TQueryCriteriaFormatter : class, IQueryCriteriaFormatter;
        public IQueryServicesBuilder WithQueryCriteriaParser<TQueryCriteriaParser>() where TQueryCriteriaParser : class, IQueryCriteriaParser;
        public IQueryServicesBuilder WithQueryExecutor<TQueryExecutor>() where TQueryExecutor : class, IQueryExecutor;
        public IQueryServicesBuilder WithLifetime(ServiceLifetime lifetime);
    }

    private class QueryServicesBuilder : IQueryServicesBuilder
    {
        ServiceLifetime lifetime = ServiceLifetime.Scoped;

        Type queryCriteriaHandlerType = typeof(QueryCriteriaHandler);
        Type expressionBuilderType = typeof(ExpressionBuilder);
        Type queryCriteriaFormatterType = typeof(ODataFormatter);
        Type queryCriteriaParserType = typeof(ODataParser);
        Type queryExecutorType = typeof(DefaultQueryExecutor);

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
