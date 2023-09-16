using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Infra;

/// <summary>
/// 외부 오픈용
/// </summary>
public abstract class JUnverifiedControllerBase : JControllerBase
{
    protected JUnverifiedControllerBase() : base()
    {
    }
}

public abstract class JUnverifiedControllerBase<TDbContext> : JControllerBase<TDbContext>
    where TDbContext : DbContext
{
    protected JUnverifiedControllerBase() : base()
    {
    }
}