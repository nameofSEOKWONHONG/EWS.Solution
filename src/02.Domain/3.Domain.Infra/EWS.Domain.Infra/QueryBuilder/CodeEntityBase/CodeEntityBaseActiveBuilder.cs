using EWS.Domain.Infra.QueryBuilder.Base;
using EWS.Infrastructure.Extentions;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.QueryBuilder.CodeEntityBase;

public class CodeEntityBaseActiveBuilder<T> : EfBuilderBase<T>
    where T : Entity.Base.CodeEntityBase
{
    private Func<T, Task> _onDelete;
    public CodeEntityBaseActiveBuilder(DbContext db, ISessionContext ctx) : base(db, ctx)
    {
    }

    public CodeEntityBaseActiveBuilder<T> SetQueryable(Func<IQueryable<T>, IQueryable<T>> onQueryable)
    {
        this.Queryable = onQueryable;
        return this;
    }

    public CodeEntityBaseActiveBuilder<T> OnDelete(Func<T, Task> onDelete)
    {
        this._onDelete = onDelete;
        return this;
    }
    
    public async Task<bool> ExecuteAsync()
    {
        var db = this.DbContext.Set<T>();
        var query = this.DbContext.Set<T>().Where(m => m.TenantId == Context.TenantId);
        
        if(this.Queryable.xIsNotEmpty())
            query = this.Queryable(query);
        
        var exist = await query.FirstOrDefaultAsync();
        if (exist.xIsEmpty()) return false;

        if (exist.IsSaveState.xIsFalse())
        {
            await this._onDelete(exist);
            db.Remove(exist);
        }
        else
        {
            exist.IsSaveState = !exist.IsSaveState;
            exist.LastModifiedBy = this.Context.UserId;
            exist.LastModifiedName = this.Context.UserName.vToAESEncrypt();
            exist.LastModifiedOn = this.Context.CurrentTimeAccessor.Now;
            db.Update(exist);
        }
        
        await this.DbContext.SaveChangesAsync();

        return true;
    }
}

public static class CodeEntityBaseActiveBuilderExtensions
{
    public static CodeEntityBaseActiveBuilder<T> CreateActiveBuilder<T>(this DbContext db, ISessionContext ctx)
        where T : Entity.Base.CodeEntityBase
    {
        return new CodeEntityBaseActiveBuilder<T>(db, ctx);
    }
}