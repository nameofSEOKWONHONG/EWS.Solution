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

public class GetResourceService: ServiceImplBase<GetResourceService, JCodeRequestBase, IResultBase<Entity.Resource>>, IGetResourceService
{
    public GetResourceService(EWSMsDbContext db, ISessionContext ctx) : base(db, ctx)
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        var result = await this.Db.CreateSelectBuilder<Entity.Resource>(this.Context)
            .SetRequest(this.Request)
            .SetQueryable(query => query)
            .ToFirstAsync();

        this.Result = await JResult<Entity.Resource>.SuccessAsync(result);
    }
}