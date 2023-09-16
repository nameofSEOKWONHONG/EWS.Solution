using System.Data;
using EWS.Application.Language;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;

namespace EWS.Domain.Infra;

/// <summary>
/// 공통
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class JControllerBase : ControllerBase
{
    protected ILogger Logger => Log.Logger;
    protected ILocalizer Localizer => HttpContext.RequestServices.GetRequiredService<ILocalizer>();
    
    
    protected JControllerBase()
    {
    }
}

public abstract class JControllerBase<TDbContext> : JControllerBase
where TDbContext : DbContext
{
    protected TDbContext Db => HttpContext.RequestServices.GetRequiredService<TDbContext>();
} 