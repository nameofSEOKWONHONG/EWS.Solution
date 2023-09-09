using EWS.Domain.Abstraction.Resource;
using EWS.Domain.Base;
using EWS.Domain.Infra.QueryBuilder.CodeEntityBase;
using EWS.Domain.Resource;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Resource;

public class GetAllResourceService : ScopeServiceImpl<GetAllResourceService, GetAllResourceRequest, JPaginatedResult<ResourceResult>>, IGetAllResourceService
{
    public GetAllResourceService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(DbContext dbContext, ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(DbContext dbContext, ISessionContext context)
    {
        this.Result = await dbContext.CreateSelectBuilder<Entity.Resource>(context)
            .SetRequest(this.Request)
            .SetQueryable(query => query)
            .ToPaginationAsync<ResourceResult>(res => res.Adapt<List<ResourceResult>>());
    }
}