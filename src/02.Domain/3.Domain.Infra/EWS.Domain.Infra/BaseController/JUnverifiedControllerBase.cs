using Microsoft.AspNetCore.Http;

namespace EWS.Domain.Infra;

/// <summary>
/// 외부 오픈용
/// </summary>
public abstract class JUnverifiedControllerBase : JControllerBase
{
    protected JUnverifiedControllerBase(IHttpContextAccessor accessor) : base(accessor)
    {
    }
}