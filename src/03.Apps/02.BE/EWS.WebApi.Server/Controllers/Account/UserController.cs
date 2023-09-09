using System.Transactions;
using EWS.Domain.Abstraction.Users;
using EWS.Domain.Account;
using EWS.Domain.Account.Users;
using EWS.Domain.Base;
using EWS.Domain.Infra;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Implement.Routers;
using eXtensionSharp;
using Microsoft.AspNetCore.Mvc;

namespace EWS.WebApi.Server.Controllers.Account;

/// <summary>
/// 접속한 유저에 대한 정보 설정
/// </summary>
public class UserController : JSessionControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="accessor"></param>
    public UserController(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("getall")]
    public async Task<IActionResult> GetAll([FromQuery]GetAllUsersRequest request)
    {
        JPaginatedResult<UserResult> result = null;
        
        using var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor);
        sr.Register<IGetAllUsersService, GetAllUsersRequest, JPaginatedResult<UserResult>>()
            .AddFilter(() => request.xIsNotEmpty())
            .SetParameter(() => request)
            .Executed(res => result = res);
        await sr.ExecuteAsync();
        
        return Ok(result);
    }
}