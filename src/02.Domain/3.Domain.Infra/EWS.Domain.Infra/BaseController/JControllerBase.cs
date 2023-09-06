using System.Transactions;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.ServiceRouter.Implement.Routers;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra;

/// <summary>
/// 공통
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class JControllerBase : ControllerBase
{
    protected readonly IHttpContextAccessor Accessor;

    protected JControllerBase(IHttpContextAccessor accessor)
    {
        this.Accessor = accessor;
    }
}