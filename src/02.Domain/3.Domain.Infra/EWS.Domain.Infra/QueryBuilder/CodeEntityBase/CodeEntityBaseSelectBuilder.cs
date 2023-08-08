using EWS.Domain.Base;
using EWS.Domain.Infra.QueryBuilder.Base;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using EWS.Infrastructure.Extentions;

namespace EWS.Domain.Infra.QueryBuilder.CodeEntityBase;

public class CodeEntityBaseSelectBuilder<T>: EfSelectBuilderBase<T>
    where T : Entity.Base.CodeEntityBase
{
    private JRequestBase _requestBase;
    public CodeEntityBaseSelectBuilder(DbContext db, ISessionContext ctx) : base(db, ctx)
    {
        
    }

    public CodeEntityBaseSelectBuilder<T> SetRequest(JRequestBase requestBase)
    {
        _requestBase = requestBase;
        return this;
    }

    public CodeEntityBaseSelectBuilder<T> SetQueryable(Func<IQueryable<T>, IQueryable<T>> onQueryable)
    {
        this.Queryable = onQueryable;
        return this;
    }

    public override async Task<T> ToFirstAsync()
    {
        var query = this.DbContext.Set<T>().Where(m => m.TenantId == Context.TenantId);
        query = this.Queryable(query);
        var result = await query.FirstOrDefaultAsync();
        return base.ToDecrypt(result);
    }

    public override async Task<TConvert> ToFirstAsync<TConvert>(Func<T, TConvert> func)
    {
        var result = await this.ToFirstAsync();
        if (this.Context.IsDecrypt)
        {
            result.CreatedName = result.CreatedName.vToAESDecrypt();
        }
        return func(result);
    }

    public override async Task<IEnumerable<T>> ListAsync()
    {
        var query = this.DbContext.Set<T>().Where(m => m.TenantId == Context.TenantId);
        query = this.Queryable(query);
        query = query.OrderByDescending(m => m.CreatedOn);
        var result = await query.ToListAsync();
        if (this.Context.IsDecrypt)
        {
            result.xForEach(item =>
            {
                item.CreatedName = item.CreatedName.vToAESDecrypt();
            });
        }
        return base.ToDecrypt(result);
    }

    public override async Task<IEnumerable<TConvert>> ToListAsync<TConvert>(Func<IEnumerable<T>, IEnumerable<TConvert>> func)
    {
        var result = await this.ListAsync();
        return func(result);
    }

    public override async Task<JPaginatedResult<T>> ToPaginationAsync()
    {
        if (_requestBase.xIsEmpty()) throw new Exception("RequestBase is empty");
        var query = this.DbContext.Set<T>().Where(m => m.TenantId == Context.TenantId);
        if (this.Queryable.xIsNotEmpty())
        {
            query = this.Queryable(query);
        }
        var pageNumber = _requestBase.PageNumber == 0 ? 1 : _requestBase.PageNumber + 1;
        var pageSize = _requestBase.PageSize == 0 ? 10 : _requestBase.PageSize;
        int count = await query.CountAsync();
        
        if (_requestBase.Orderby.xIsNotEmpty())
        {
            query = query.OrderBy($"{_requestBase.Sort} {_requestBase.Orderby}" );
        }
        else
        {
            query = query.OrderByDescending(m => m.CreatedOn);
        }
        List<T> items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        if (this.Context.IsDecrypt)
        {
            items.xForEach(item =>
            {
                item.CreatedName = item.CreatedName.vToAESDecrypt();
            });
        }
        return JPaginatedResult<T>.Success(items, count, pageNumber, pageSize);
    }
    
    public override async Task<JPaginatedResult<TConvert>> ToPaginationAsync<TConvert>(Func<IEnumerable<T>, IEnumerable<TConvert>> func)
    {
        var result = await this.ToPaginationAsync();
        var converted = func(result.Data);
        return await JPaginatedResult<TConvert>.SuccessAsync(converted.ToList(), result.TotalCount, result.CurrentPage, result.PageSize);
    }   
}

public static class CodeEntityBaseSelectBuilderExtensions
{
    public static CodeEntityBaseSelectBuilder<T> CreateSelectBuilder<T>(this DbContext db, ISessionContext ctx)
        where T : Entity.Base.CodeEntityBase
    {
        return new CodeEntityBaseSelectBuilder<T>(db, ctx);
    }
}