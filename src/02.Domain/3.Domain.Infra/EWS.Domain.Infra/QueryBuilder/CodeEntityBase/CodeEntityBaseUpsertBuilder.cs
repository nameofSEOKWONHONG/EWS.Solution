using EWS.Domain.Infra.QueryBuilder.Base;
using EWS.Infrastructure.Extentions;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.QueryBuilder.CodeEntityBase;

public class CodeEntityBaseUpsertBuilder<T> : EfUpsertBuilderBase<T>
    where T : Entity.Base.CodeEntityBase
{
    private Func<T> _addFunc;
    private Func<T, T> _updateFunc;
    public CodeEntityBaseUpsertBuilder(DbContext db, ISessionContext ctx) : base(db, ctx)
    {
    }
    
    public CodeEntityBaseUpsertBuilder<T> SetQueryable(Func<IQueryable<T>, IQueryable<T>> onQueryable)
    {
        this.Queryable = onQueryable;
        return this;
    }
    
    public CodeEntityBaseUpsertBuilder<T> OnAdd(Func<T> addFunc)
    {
        _addFunc = addFunc;
        return this;
    }

    public CodeEntityBaseUpsertBuilder<T> OnUpdate(Func<T, T> updateFunc)
    {
        _updateFunc = updateFunc;
        return this;
    }

    public override async Task<T> ExecuteAsync()
    {
        var db = this.DbContext.Set<T>();
        var query = this.DbContext.Set<T>().Where(m => m.TenantId == Context.TenantId);
        
        if(this.Queryable.xIsNotEmpty())
            query = this.Queryable(query);
        
        var exist = await query.FirstOrDefaultAsync();
        if (exist.xIsEmpty())
        {
            exist = _addFunc();
            exist.TenantId = this.Context.TenantId;
            exist.CreatedBy = this.Context.UserId;
            exist.CreatedName = this.Context.UserName.vToAESEncrypt();
            exist.CreatedOn = this.Context.CurrentTimeAccessor.Now;
            await db.AddAsync(exist);
        }
        else
        {
            exist = _updateFunc(exist);
            exist.TenantId = this.Context.TenantId;
            exist.LastModifiedBy = this.Context.UserId;
            exist.LastModifiedName = this.Context.UserName.vToAESEncrypt();
            exist.LastModifiedOn = this.Context.CurrentTimeAccessor.Now;
            db.Update(exist);
        }

        await this.DbContext.SaveChangesAsync();
        this.DbContext.ChangeTracker.Clear();
        return exist;
    }
}

public static class CodeEntityBaseUpsertBuilderExtensions
{
    public static CodeEntityBaseUpsertBuilder<T> CreateUpsertBuilder<T>(this DbContext db, ISessionContext ctx)
        where T : Entity.Base.CodeEntityBase
    {
        return new CodeEntityBaseUpsertBuilder<T>(db, ctx);
    }
}
