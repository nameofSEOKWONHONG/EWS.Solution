using EWS.Entity;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.QueryBuilder.Base;

internal class EfActiveBuilderBase<TEntity>: EfBuilderBase<TEntity>
    where TEntity : Entity.EntityBase
{
    public EfActiveBuilderBase(DbContext db, ISessionContext ctx) : base(db, ctx)
    {
        db.Database.ExecuteSqlRaw("SET NOCOUNT OFF; ");
    }
    
    public EfActiveBuilderBase<TEntity> SetQueryable(Func<IQueryable<TEntity>, IQueryable<TEntity>> onQueryable)
    {
        this.Queryable = onQueryable;
        return this;
    }
    
    public virtual async Task ExecuteAsync()
    {
        var db = this.DbContext.Set<TEntity>();
        var query = this.DbContext.Set<TEntity>().Where(m => m.TenantId == this.Context.TenantId);
        if (this.Queryable.xIsNotEmpty())
        {
            query = this.Queryable(query);
        }

        var exist = await query.FirstOrDefaultAsync();
        
        if (exist.IsActive.xIsTrue())
        {
            exist.IsActive = !exist.IsActive;
            db.Update(exist);
            await this.DbContext.SaveChangesAsync();
        }
        else
        {
            db.Remove(exist);
            await this.DbContext.SaveChangesAsync();
        }
        
        this.DbContext.ChangeTracker.Clear();
    }
}

public static class EfActiveBuilderExtensions
{
    internal static EfActiveBuilderBase<T> CreateActiveBuilder<T>(this DbContext db, ISessionContext ctx)
        where T : EntityBase
    {
        return new EfActiveBuilderBase<T>(db, ctx);
    }
}