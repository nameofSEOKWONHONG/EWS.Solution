using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EWS.Domain.Infra;

/// <summary>
/// login 이후
/// </summary>
//[Authorize]
public abstract class JSessionControllerBase : JControllerBase
{
    protected JSessionControllerBase(IHttpContextAccessor accessor) : base(accessor)
    {
    }
}