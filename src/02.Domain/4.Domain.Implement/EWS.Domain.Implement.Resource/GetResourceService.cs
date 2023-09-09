﻿using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Resource;
using EWS.Domain.Base;
using EWS.Domain.Infra.QueryBuilder.CodeEntityBase;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Resource;

public class GetResourceService: ScopeServiceImpl<GetResourceService, JCodeRequestBase, IResultBase<Entity.Resource>>, IGetResourceService
{
    public GetResourceService(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(DbContext dbContext, ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(DbContext dbContext, ISessionContext context)
    {
        var result = await dbContext.CreateSelectBuilder<Entity.Resource>(context)
            .SetRequest(this.Request)
            .SetQueryable(query => query)
            .ToFirstAsync();

        this.Result = await JResult<Entity.Resource>.SuccessAsync(result);
    }
}