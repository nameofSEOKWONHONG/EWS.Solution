using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Resource;
using EWS.Domain.Base;
using EWS.Domain.Infra.QueryBuilder.CodeEntityBase;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;

namespace EWS.Domain.Implement.Resource;

public class SaveResourceService: ScopeServiceImpl<GetResourceService, Entity.Resource, IResultBase<Entity.Resource>>, ISaveResourceService
{
    public SaveResourceService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        var result = await this.DbContext.CreateUpsertBuilder<Entity.Resource>(context)
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