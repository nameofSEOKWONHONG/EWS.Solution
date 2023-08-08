using EWS.Domain.Infra.Extension;
using EWS.Entity.Base;
using EWS.Infrastructure.Extentions;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra.QueryBuilder.Base;

public abstract class EfBuilderBase<TEntity>
where TEntity : IEntityBase
{
    protected readonly DbContext DbContext;
    protected readonly ISessionContext Context;
    protected Func<IQueryable<TEntity>, IQueryable<TEntity>> Queryable;
    
    protected EfBuilderBase(DbContext dbContext, ISessionContext ctx)
    {
        DbContext = dbContext;
        Context = ctx;
    }
    
    protected TEntity ToDecrypt(TEntity entity)
    {
        if (entity.xIsEmpty()) return default;
        
        if (this.Context.IsDecrypt)
        {
            entity.CreatedName.vToAESDecrypt();
            entity.LastModifiedName.vToAESDecrypt();
        }

        return entity;
    }

    protected IEnumerable<TEntity> ToDecrypt(IEnumerable<TEntity> entities)
    {
        if (entities.xIsEmpty()) return null;
        
        foreach (var entity in entities)
        {
            entity.CreatedName.vToAESDecrypt();
            entity.LastModifiedName.vToAESDecrypt();
        }

        return entities;
    }
}