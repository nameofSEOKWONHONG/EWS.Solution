using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra;

/// <summary>
/// login 이후
/// </summary>
[Authorize]
public abstract class JSessionControllerBase : JControllerBase
{
    protected JSessionControllerBase() : base()
    {
    }
}

public abstract class JSessionControllerBase<TDbContext> : JControllerBase<TDbContext>
    where TDbContext : DbContext
{
    protected JSessionControllerBase() : base()
    {
    }
}