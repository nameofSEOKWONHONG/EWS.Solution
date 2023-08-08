using EWS.Domain.Base;
using EWS.Domain.Common;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.Extension;

public static class QueryableExtensions
{
    public static (int Skip, int PageSize) vToPaging(this JRequestBase request)
    {
        request.PageNumber = request.PageNumber == 0 ? 1 : request.PageNumber + 1;
        request.PageSize = request.PageSize == 0 ? 10 : request.PageSize;
        var skip = (request.PageNumber - 1) * request.PageSize;
        return new(skip, request.PageSize);
    }
    
    public static async Task<JPaginatedResult<T>> vToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize) where T : class
    {
        if (source == null) throw new Exception("source is null");
        pageNumber = pageNumber == 0 ? 1 : pageNumber + 1;
        pageSize = pageSize == 0 ? 10 : pageSize;
        int count = await source.CountAsync();
        List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return await JPaginatedResult<T>.SuccessAsync(items, count, pageNumber, pageSize);
    }

    public static async Task<T> vToFirstAsync<T>(this IQueryable<T> source)
        where T : class, new()
    {
        var result = await source.FirstOrDefaultAsync();
        return result.xIsEmpty() ? new T() : result;
    }
    
    public static async Task<List<T>> vToListAsync<T>(this IQueryable<T> source)
        where T : class
    {
        var result = await source.ToListAsync();
        return result.xIsEmpty() ? new List<T>() : result;
    }
}