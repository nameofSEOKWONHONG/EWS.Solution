using EWS.Application.Language;
using EWS.Application.Wrapper;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Base;
using EWS.Domain.Identity;
using EWS.Domain.Infra;
using EWS.Domain.Infrastructure;
using EWS.Entity.Db;
using eXtensionSharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EWS.WebApi.Server.Controllers.Account;

/// <summary>
/// 접근에 대한 정보
/// </summary>
public class TokenController : JControllerBase<EWSMsDbContext>
{
    /// <summary>
    /// 
    /// </summary>
    public TokenController()
    {
        
    }
    
    /// <summary>
    /// jwt token 조회
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Get([FromServices]ITokenService service,
        [FromServices]ILocalizer localizer,
        TokenRequest model)
    {
        IResultBase<TokenResponse> result = null;
        await service.Create<ITokenService, TokenRequest, IResultBase<TokenResponse>>()
            .UseTransaction(this.Db)
            .AddFilter(model.xIsNotEmpty)
            .SetParameter(() => model)
            .SetValidator(new TokenRequest.Validator(localizer))
            .OnValidated(v =>
            {
                result = JResult<TokenResponse>.Fail(v.Errors.First().ErrorMessage);
            })
            .OnExecuted(r =>
            {
                result = r;
            });
        
        return Ok(result);
    }
    
    /// <summary>
    /// jwt token 갱신
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh([FromServices]ITokenRefreshService service,
        [FromBody] RefreshTokenRequest model)
    {
        IResultBase<TokenResponse> result = null;
        await service.Create<ITokenRefreshService, RefreshTokenRequest, IResultBase<TokenResponse>>()
            .UseTransaction(this.Db)
            .AddFilter(model.xIsNotEmpty)
            .SetParameter(() => model)
            .SetValidator(new RefreshTokenRequest.Valdiator(Localizer))
            .OnValidated(m =>
            {
                result = JResult<TokenResponse>.Fail(m.Errors.First().ErrorMessage);
            })
            .OnExecuted(m =>
            {
                result = m;
            });
        
        return Ok(result);
    }    
}