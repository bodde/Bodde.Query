using System.Linq.Expressions;
using Bodde.Query.Abstractions.Models;
using Bodde.Query.Abstractions.Services;
using Bodde.Query.Core;

namespace Bodde.Query.Test;

public class DefaultQueryToolkit_ctor
{
    [Fact]
    public async Task Default_Tools_AreAvailable()
    {
        var sut = QueryToolkit.Default();

        var formatter = sut.Formatter;
        var parser = sut.Parser;
        var handler = sut.Handler;
        var executor = sut.Executor;
        var expressionBuilder = sut.ExpressionBuilder;

        Assert.NotNull(formatter);
        Assert.IsType<ODataFormatter>(formatter);

        Assert.NotNull(parser);
        Assert.IsType<ODataParser>(parser);

        Assert.NotNull(handler);
        Assert.IsType<QueryCriteriaHandler>(handler);

        Assert.NotNull(executor);
        Assert.IsType<DefaultQueryExecutor>(executor);

        Assert.NotNull(expressionBuilder);
        Assert.IsType<ExpressionBuilder>(expressionBuilder);
    }

        [Fact]
    public async Task Custom_Tools_AreAvailable()
    {
        var sut = new QueryToolkit(
            new CustomFormatter(), 
            new CustomParser(), 
            new CustomExpressionBuilder(), 
            new CustomHandler(), 
            new CustomExecutor()
            );

        var formatter = sut.Formatter;
        var parser = sut.Parser;
        var handler = sut.Handler;
        var executor = sut.Executor;
        var expressionBuilder = sut.ExpressionBuilder;

        Assert.NotNull(formatter);
        Assert.IsType<CustomFormatter>(formatter);

        Assert.NotNull(parser);
        Assert.IsType<CustomParser>(parser);

        Assert.NotNull(handler);
        Assert.IsType<CustomHandler>(handler);

        Assert.NotNull(executor);
        Assert.IsType<CustomExecutor>(executor);

        Assert.NotNull(expressionBuilder);
        Assert.IsType<CustomExpressionBuilder>(expressionBuilder);
    }

    private class CustomParser : IQueryCriteriaParser
    {
        public QueryCriteria Parse(string criteriaString)
        {
            throw new NotImplementedException();
        }

        public QueryCriteria Parse(string? filter = null, string? orderBy = null, int? skip = null, int? top = null, bool? totalCount = null)
        {
            throw new NotImplementedException();
        }

        public QueryCriteria Parse(QueryCriteriaParameters queryCriteriaParameters)
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

    private class CustomFormatter : IQueryCriteriaFormatter
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

    private class CustomHandler : IQueryCriteriaHandler
    {
        public IQueryable<T> ApplyCriteria<T>(IQueryable<T> query, QueryCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public QueryCriteriaResult<T> ToResult<T>(QueryableWithCriteria<T> query)
        {
            throw new NotImplementedException();
        }

        public Task<QueryCriteriaResult<T>> ToResultAsync<T>(QueryableWithCriteria<T> query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private class CustomExecutor : IQueryExecutor
    {
        public Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<T[]> ToArrayAsync<T>(IQueryable<T> query, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public QueryCriteriaResult<T> ToResult<T>(QueryableWithCriteria<T> query)
        {
            throw new NotImplementedException();
        }

        public Task<QueryCriteriaResult<T>> ToResultAsync<T>(QueryableWithCriteria<T> query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}