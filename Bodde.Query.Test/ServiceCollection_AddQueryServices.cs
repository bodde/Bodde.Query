using System.Linq.Expressions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;
using Bodde.Query.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Bodde.Query.Test;

public class ServiceCollection_AddQueryServices
{
    private readonly ServiceCollection sut;

    public ServiceCollection_AddQueryServices()
    {        
        sut = new ServiceCollection();
    }

	[Fact]
    public void With_DefaultBuilder()
    {
        sut.AddQueryServices();

        var queryCriteriaHandler = sut.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryCriteriaHandler));
        var expressionBuilder = sut.FirstOrDefault(sd => sd.ServiceType == typeof(IExpressionBuilder));
        var queryCriteriaFormatter = sut.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryCriteriaFormatter));
        var queryCriteriaParser = sut.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryCriteriaParser));
        var queryExecutor = sut.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryExecutor));

        Assert.NotNull(queryCriteriaHandler);
        Assert.Equal(ServiceLifetime.Scoped, queryCriteriaHandler.Lifetime);
        Assert.Equal(typeof(QueryCriteriaHandler), queryCriteriaHandler.ImplementationType);

        Assert.NotNull(expressionBuilder);
        Assert.Equal(ServiceLifetime.Scoped, expressionBuilder.Lifetime);
        Assert.Equal(typeof(ExpressionBuilder), expressionBuilder.ImplementationType);

        Assert.NotNull(queryCriteriaFormatter);
        Assert.Equal(ServiceLifetime.Scoped, queryCriteriaFormatter.Lifetime);
        Assert.Equal(typeof(ODataFormatter), queryCriteriaFormatter.ImplementationType);

        Assert.NotNull(queryCriteriaParser);
        Assert.Equal(ServiceLifetime.Scoped, queryCriteriaParser.Lifetime);
        Assert.Equal(typeof(ODataParser), queryCriteriaParser.ImplementationType);

        Assert.NotNull(queryExecutor);
        Assert.Equal(ServiceLifetime.Scoped, queryExecutor.Lifetime);
        Assert.Equal(typeof(DefaultQueryExecutor), queryExecutor.ImplementationType);
    }

	[Fact]
    public void WithLifetime_Scoped()
    {
        sut.AddQueryServices(builder => builder.WithLifetime(ServiceLifetime.Scoped));

        foreach(var serviceDescriptor in sut)
        {
            Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
        }
    }

	[Fact]
    public void WithLifetime_Singleton()
    {
        sut.AddQueryServices(builder => builder.WithLifetime(ServiceLifetime.Singleton));

        foreach(var serviceDescriptor in sut)
        {
            Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
        }
    }

	[Fact]
    public void WithLifetime_Transient()
    {
        sut.AddQueryServices(builder => builder.WithLifetime(ServiceLifetime.Transient));

        foreach(var serviceDescriptor in sut)
        {
            Assert.Equal(ServiceLifetime.Transient, serviceDescriptor.Lifetime);
        }
    }

    [Fact]
    public void WithQueryCriteriaHandler_Custom()
    {
        sut.AddQueryServices(builder => builder.WithQueryCriteriaHandler<CustomQueryCriteriaHandler>());

        var actual = sut.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryCriteriaHandler));

        Assert.NotNull(actual);
        Assert.Equal(typeof(CustomQueryCriteriaHandler), actual.ImplementationType);
    }

    [Fact]
    public void WithExpressionBuilder_Custom()
    {
        sut.AddQueryServices(builder => builder.WithExpressionBuilder<CustomExpressionBuilder>());

        var actual = sut.FirstOrDefault(sd => sd.ServiceType == typeof(IExpressionBuilder));

        Assert.NotNull(actual);
        Assert.Equal(typeof(CustomExpressionBuilder), actual.ImplementationType);
    }

    [Fact]
    public void WithQueryCriteriaFormatter_Custom()
    {
        sut.AddQueryServices(builder => builder.WithQueryCriteriaFormatter<CustomQueryCriteriaFormatter>());

        var actual = sut.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryCriteriaFormatter));

        Assert.NotNull(actual);
        Assert.Equal(typeof(CustomQueryCriteriaFormatter), actual.ImplementationType);
    }

    [Fact]
    public void WithQueryCriteriaParser_Custom()
    {
        sut.AddQueryServices(builder => builder.WithQueryCriteriaParser<CustomQueryCriteriaParser>());

        var actual = sut.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryCriteriaParser));

        Assert.NotNull(actual);
        Assert.Equal(typeof(CustomQueryCriteriaParser), actual.ImplementationType);
    }

    [Fact]
    public void WithQueryExecutor_Custom()
    {
        sut.AddQueryServices(builder => builder.WithQueryExecutor<CustomQueryExecutor>());

        var actual = sut.FirstOrDefault(sd => sd.ServiceType == typeof(IQueryExecutor));

        Assert.NotNull(actual);
        Assert.Equal(typeof(CustomQueryExecutor), actual.ImplementationType);
    }

    private class CustomQueryCriteriaHandler : IQueryCriteriaHandler
    {
        public QueryCriteriaQueryable<T> ApplyCriteria<T>(IQueryable<T> query, QueryCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public Task<QueryCriteriaResult<T>> ToResultAsync<T>(QueryCriteriaQueryable<T> query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<QueryCriteriaResult<T>> ToResultAsync<T>(IQueryable<T> query, QueryCriteria criteria, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private class CustomExpressionBuilder : IExpressionBuilder
    {
        public Expression<Func<T, bool>> CreateFilterExpression<T>(FilterCriteria.FilterExpression filterExpression)
        {
            throw new NotImplementedException();
        }

        public ParameterExpression CreateParameterExpression<T>()
        {
            throw new NotImplementedException();
        }

        public Expression<Func<T, object>> CreatePropertyOrFieldExpressionFromPath<T>(string propertyPath, ParameterExpression? parameter = null)
        {
            throw new NotImplementedException();
        }
    }

    private class CustomQueryCriteriaFormatter : IQueryCriteriaFormatter
    {
        public string Format(QueryCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public string FormatFilter(FilterCriteria filter)
        {
            throw new NotImplementedException();
        }

        public string FormatOrderBy(OrderByCriteria orderBy)
        {
            throw new NotImplementedException();
        }

        public string FormatPaging(PagingCriteria paging)
        {
            throw new NotImplementedException();
        }
    }

    private class CustomQueryCriteriaParser : IQueryCriteriaParser
    {
        public QueryCriteria Parse(string criteriaString)
        {
            throw new NotImplementedException();
        }

        public QueryCriteria Parse(QueryCriteriaParameters? queryCriteriaParameters)
        {
            throw new NotImplementedException();
        }

        public QueryCriteria Parse(string? filter = null, string? orderBy = null, int? skip = null, int? top = null, bool? totalCount = null)
        {
            throw new NotImplementedException();
        }

        public FilterCriteria ParseFilter(string filterString)
        {
            throw new NotImplementedException();
        }

        public FilterCriteria.FilterExpression ParseFilterExpression(string filterString)
        {
            throw new NotImplementedException();
        }

        public OrderByCriteria ParseOrderBy(string orderByString)
        {
            throw new NotImplementedException();
        }

        public OrderByCriteria.OrderByItem[] ParseOrderByItems(string orderByString)
        {
            throw new NotImplementedException();
        }

        public PagingCriteria ParsePaging(string pagingString)
        {
            throw new NotImplementedException();
        }
    }
    
    private class CustomQueryExecutor : IQueryExecutor
    {
        public Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}