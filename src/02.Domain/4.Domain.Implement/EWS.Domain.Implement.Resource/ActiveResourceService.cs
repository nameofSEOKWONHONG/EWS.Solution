using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Resource;
using EWS.Domain.Base;
using EWS.Domain.Infra.QueryBuilder.CodeEntityBase;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Resource;

public class ActiveResourceService: ServiceImplBase<GetResourceService, Entity.Resource, IResultBase>, IActiveResourceService
{
    public ActiveResourceService(EWSMsDbContext db, ISessionContext ctx) : base(db, ctx)
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        var result =  await this.Db.CreateActiveBuilder<Entity.Resource>(this.Context)
            .OnDelete(async exist =>
            {
                var db = Db.Set<SubResource>();
                var items = await db.AsNoTracking().Where(m => m.TenantId == Context.TenantId &&
                                                m.Resource.TenantId == Context.TenantId &&
                                                m.Resource.Code == exist.Code).ToListAsync();
                db.RemoveRange(items);
                await Db.SaveChangesAsync();
            })
            .ExecuteAsync();
        
        if (result.xIsTrue()) this.Result = await JResult.SuccessAsync();
        this.Result = await JResult.FailAsync();
    }
}