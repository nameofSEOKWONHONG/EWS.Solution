using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Resource;
using EWS.Domain.Base;
using EWS.Domain.Infra.QueryBuilder.CodeEntityBase;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Resource;

public class SaveResourceService: ServiceImplBase<GetResourceService, Entity.Resource, IResultBase<Entity.Resource>>, ISaveResourceService
{
    public SaveResourceService(EWSMsDbContext db, ISessionContext ctx) : base(db, ctx)
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        var result = await this.Db.CreateUpsertBuilder<Entity.Resource>(this.Context)
            .OnAdd(() => this.Request)
            .OnUpdate(exist =>
            {
                exist.Name = this.Request.Name;
                exist.Description = this.Request.Description;
                return exist;
            })
            .ExecuteAsync();
        
        this.Result = await JResult<Entity.Resource>.SuccessAsync(result);
    }
}