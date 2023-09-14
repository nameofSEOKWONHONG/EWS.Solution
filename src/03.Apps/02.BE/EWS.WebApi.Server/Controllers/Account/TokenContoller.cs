using System.Transactions;
using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Identity;
using EWS.Domain.Infra;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Implement.Routers;
using eXtensionSharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWS.WebApi.Server.Controllers.Account;

/// <summary>
/// 접근에 대한 정보
/// </summary>
public class TokenController : JControllerBase
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="accessor"></param>
    public TokenController(IHttpContextAccessor accessor) : base(accessor)
    {
    }
    
    /// <summary>
    /// jwt token 조회
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Get(TokenRequest model)
    {
        IResultBase<TokenResponse> result = null;
        using (var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor))
        {
            sr.Register<ITokenService, TokenRequest, IResultBase<TokenResponse>>()
                .AddFilter(() => model.xIsNotEmpty())
                .SetParameter(() => model)
                .Executed(res => result = res);

            await sr.ExecuteAsync();
        }
        return Ok(result);
    }
    
    /// <summary>
    /// jwt token 갱신
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest model)
    {
        IResultBase<TokenResponse> result = null;
        
        using var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor);
        sr.Register<ITokenRefreshService, RefreshTokenRequest, IResultBase<TokenResponse>>()
            .AddFilter(() => model.xIsNotEmpty())
            .SetParameter(() => model)
            .Executed(res => result = res);
        await sr.ExecuteAsync();
        
        return Ok(result);
    }    
}