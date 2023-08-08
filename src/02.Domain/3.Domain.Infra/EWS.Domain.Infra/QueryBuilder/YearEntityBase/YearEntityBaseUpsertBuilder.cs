using EWS.Domain.Infra.QueryBuilder.Base;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.QueryBuilder.YearEntityBase;

public class YearEntityBaseUpsertBuilder<T> : EfUpsertBuilderBase<T>
    where T : Entity.Base.YearEntityBase
{
    private Func<Task<T>> _addFunc;
    private Func<T, Task<T>> _updateFunc;
    public YearEntityBaseUpsertBuilder(DbContext db, ISessionContext ctx) : base(db, ctx)
    {
    }
    
    public YearEntityBaseUpsertBuilder<T> SetQueryable(Func<IQueryable<T>, IQueryable<T>> onQueryable)
    {
        this.Queryable = onQueryable;
        return this;
    }

    
    public YearEntityBaseUpsertBuilder<T> OnAddAsync(Func<Task<T>> addFunc)
    {
        _addFunc = addFunc;
        return this;
    }

    public YearEntityBaseUpsertBuilder<T> OnUpdateAsync(Func<T, Task<T>> updateFunc)
    {
        _updateFunc = updateFunc;
        return this;
    }

    public override async Task<T> ExecuteAsync()
    {
        var db = this.DbContext.Set<T>();
        var query = this.DbContext.Set<T>().Where(m => m.TenantId == Context.TenantId);
        query = this.Queryable(query);
        var exist = await query.FirstOrDefaultAsync();
        if (exist.xIsEmpty())
        {
            exist = await _addFunc();
            exist.TenantId = this.Context.TenantId;
            await db.AddAsync(exist);
        }
        else
        {
            exist = await _updateFunc(exist);
            exist.TenantId = this.Context.TenantId;
            db.Update(exist);
        }

        await this.DbContext.SaveChangesAsync();
        this.DbContext.ChangeTracker.Clear();
        return exist;
    }
}

public static class YearEntityBaseUpsertBuilderExtensions
{
    public static YearEntityBaseUpsertBuilder<T> CreateUpsertBuilder<T>(this DbContext db, ISessionContext ctx)
        where T : Entity.Base.YearEntityBase
    {
        return new YearEntityBaseUpsertBuilder<T>(db, ctx);
    }
}
