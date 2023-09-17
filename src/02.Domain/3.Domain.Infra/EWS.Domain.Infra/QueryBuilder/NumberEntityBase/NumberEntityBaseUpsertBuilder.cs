using EWS.Domain.Infra.QueryBuilder.Base;
using EWS.Domain.Infra.Session;
using EWS.Domain.Infra.Session.Accessor;
using EWS.Entity;
using EWS.Entity.Base;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.QueryBuilder.NumberEntityBase;

public class NumberEntityBaseUpsertBuilder<T> : EfUpsertBuilderBase<T>
    where T : Entity.Base.NumberEntityBase
{
    private Func<Task<T>> _addFunc;
    private Func<T, Task<T>> _updateFunc;
    public NumberEntityBaseUpsertBuilder(DbContext db, ISessionContext ctx) : base(db, ctx)
    {
    }
    
    public NumberEntityBaseUpsertBuilder<T> SetQueryable(Func<IQueryable<T>, IQueryable<T>> onQueryable)
    {
        this.Queryable = onQueryable;
        return this;
    }

    
    public NumberEntityBaseUpsertBuilder<T> OnAddAsync(Func<Task<T>> addFunc)
    {
        _addFunc = addFunc;
        return this;
    }

    public NumberEntityBaseUpsertBuilder<T> OnUpdateAsync(Func<T, Task<T>> updateFunc)
    {
        _updateFunc = updateFunc;
        return this;
    }

    public override async Task<T> ExecuteAsync()
    {
        var infraAccessor = (InfraAccessor)this.Context.InfraAccessor;
        var db = this.DbContext.Set<T>();
        var query = this.DbContext.Set<T>().Where(m => m.TenantId == Context.TenantId);
        query = this.Queryable(query);
        var exist = await query.FirstOrDefaultAsync();
        if (exist.xIsEmpty())
        {
            exist = await _addFunc();
            exist.TenantId = this.Context.TenantId;
            var now = DateTime.UtcNow;
            var seq = await infraAccessor.SequentialService.ExecuteAsync(this.DbContext, Context,
                new Sequential()
                {
                    TenantId = this.Context.TenantId,
                    TableName = XAttributeExtensions.xGetTableName<T>(),
                    Year = now.Year,
                    Month = 0,
                    Day = 0
                });            
            exist.Id = seq;
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

public static class NumberEntityBaseUpsertBuilderExtensions
{
    public static NumberEntityBaseUpsertBuilder<T> CreateUpsertBuilder<T>(this DbContext db, ISessionContext ctx)
    where T : Entity.Base.NumberEntityBase
    {
        return new NumberEntityBaseUpsertBuilder<T>(db, ctx);
    }
}