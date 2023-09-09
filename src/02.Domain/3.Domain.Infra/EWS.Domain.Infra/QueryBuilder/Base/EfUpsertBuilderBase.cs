using EWS.Infrastructure.Session.Abstract;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.QueryBuilder.Base;

public abstract class EfUpsertBuilderBase<TEntity> : EfBuilderBase<TEntity>
    where TEntity : Entity.EntityBase
{
    protected Func<IQueryable<TEntity>, IQueryable<TEntity>> Queryable;

    protected EfUpsertBuilderBase(DbContext db, ISessionContext ctx) : base(db, ctx)
    {
        db.Database.ExecuteSqlRaw("SET NOCOUNT OFF; ");
    }

    public abstract Task<TEntity> ExecuteAsync();
}