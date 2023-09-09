using System.Data;
using EWS.Domain.Base;
using EWS.Domain.Infra.Extension;
using EWS.Entity;
using EWS.Entity.Base;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

namespace EWS.Domain.Infra.QueryBuilder.Base;

public abstract class EfSelectBuilderBase<TEntity> : EfBuilderBase<TEntity>
where TEntity : EntityBase
{
    protected JRequestBase RequestBase;
    protected EfSelectBuilderBase(DbContext db, ISessionContext ctx) : base(db, ctx)
    {
        db.Database.ExecuteSqlRaw("SET NOCOUNT ON; ");
    }

    public abstract Task<TEntity> ToFirstAsync();
    
    public abstract Task<TConvert> ToFirstAsync<TConvert>(Func<TEntity, TConvert> func);
    
    public abstract Task<IEnumerable<TEntity>> ListAsync();
    
    public abstract Task<IEnumerable<TConvert>> ToListAsync<TConvert>(Func<IEnumerable<TEntity>, IEnumerable<TConvert>> func);

    public abstract Task<JPaginatedResult<TEntity>> ToPaginationAsync();

    public abstract Task<JPaginatedResult<TConvert>> ToPaginationAsync<TConvert>(Func<IEnumerable<TEntity>, IEnumerable<TConvert>> func);
}

