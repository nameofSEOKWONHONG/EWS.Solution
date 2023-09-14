using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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