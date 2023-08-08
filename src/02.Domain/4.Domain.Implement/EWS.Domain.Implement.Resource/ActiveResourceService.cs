using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Resource;
using EWS.Domain.Base;
using EWS.Domain.Infra.QueryBuilder.CodeEntityBase;
using EWS.Entity;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Resource;

public class ActiveResourceService: ScopeServiceImpl<GetResourceService, Entity.Resource, IResultBase>, IActiveResourceService
{
    public ActiveResourceService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        var result =  await this.DbContext.CreateActiveBuilder<Entity.Resource>(context)
            .OnDelete(async exist =>
            {
                var db = this.DbContext.Set<SubResource>();
                var items = await db.Where(m => m.TenantId == context.TenantId &&
                                  m.Resource.TenantId == context.TenantId &&
                                  m.Resource.Code == exist.Code).ToListAsync();
                db.RemoveRange(items);
                await this.DbContext.SaveChangesAsync();
            })
            .ExecuteAsync();
        
        if (result.xIsTrue()) this.Result = await JResult.SuccessAsync();
        this.Result = await JResult.FailAsync();
    }
}