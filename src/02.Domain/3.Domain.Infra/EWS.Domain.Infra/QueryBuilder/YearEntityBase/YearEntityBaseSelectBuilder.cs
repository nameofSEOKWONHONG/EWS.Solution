using EWS.Domain.Base;
using EWS.Domain.Infra.QueryBuilder.Base;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using EWS.Infrastructure.Extentions;

namespace EWS.Domain.Infra.QueryBuilder.YearEntityBase;

public class YearEntityBaseSelectBuilder<T> : EfSelectBuilderBase<T>
    where T : Entity.Base.YearEntityBase
{
    
    public YearEntityBaseSelectBuilder(DbContext db, ISessionContext ctx) : base(db, ctx)
    {
        
    }

    public YearEntityBaseSelectBuilder<T> SetRequest(JRequestBase requestBase)
    {
        this.RequestBase = requestBase;
        return this;
    }
    
    public YearEntityBaseSelectBuilder<T> SetQueryable(Func<IQueryable<T>, IQueryable<T>> onQueryable)
    {
        this.Queryable = onQueryable;
        return this;
    }

    public override async Task<T> ToFirstAsync()
    {
        var query = this.DbContext.Set<T>().Where(m => m.TenantId == Context.TenantId);
        query = this.Queryable(query);
        var result = await query.FirstOrDefaultAsync();
        if (this.Context.IsDecrypt)
        {
            result.CreatedName = result.CreatedName.vToAESDecrypt();
        }
        return base.ToDecrypt(result);
    }

    public override async Task<TConvert> ToFirstAsync<TConvert>(Func<T, TConvert> func)
    {
        var result = await this.ToFirstAsync();
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
        var query = this.DbContext.Set<T>().Where(m => m.TenantId == Context.TenantId);
        if (this.Queryable.xIsNotEmpty())
        {
            query = this.Queryable(query);
        }
        var pageNumber = this.RequestBase.PageNumber == 0 ? 1 : this.RequestBase.PageNumber + 1;
        var pageSize = this.RequestBase.PageSize == 0 ? 10 : this.RequestBase.PageSize;
        int count = await query.CountAsync();
        
        if (this.RequestBase.Orderby.xIsNotEmpty())
        {
            query = query.OrderBy($"{this.RequestBase.Sort} {this.RequestBase.Orderby}" );
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

public static class YearEntityBaseSelectBuilderExtensions
{
    public static YearEntityBaseSelectBuilder<T> CreateSelectBuilder<T>(this DbContext db, ISessionContext ctx)
        where T : Entity.Base.YearEntityBase
    {
        return new YearEntityBaseSelectBuilder<T>(db, ctx);
    }
}