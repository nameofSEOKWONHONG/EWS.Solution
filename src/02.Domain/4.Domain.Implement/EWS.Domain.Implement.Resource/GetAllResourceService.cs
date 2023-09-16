using EWS.Domain.Abstraction.Resource;
using EWS.Domain.Base;
using EWS.Domain.Infra.QueryBuilder.CodeEntityBase;
using EWS.Domain.Resource;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Resource;

public class GetAllResourceService : ServiceImplBase<GetAllResourceService, GetAllResourceRequest, JPaginatedResult<ResourceResult>>, IGetAllResourceService
{
    public GetAllResourceService(EWSMsDbContext db, ISessionContext ctx) : base(db, ctx)
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        this.Result = await this.Db.CreateSelectBuilder<Entity.Resource>(this.Context)
            .SetRequest(this.Request)
            .SetQueryable(query => query)
            .ToPaginationAsync<ResourceResult>(res => res.Adapt<List<ResourceResult>>());
    }
}