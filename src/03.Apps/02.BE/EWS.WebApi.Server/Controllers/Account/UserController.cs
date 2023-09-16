using EWS.Domain.Abstraction.Users;
using EWS.Domain.Account;
using EWS.Domain.Account.Users;
using EWS.Domain.Base;
using EWS.Domain.Infra;
using EWS.Domain.Infrastructure;
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
    public UserController() : base()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("getall")]
    public async Task<IActionResult> GetAll([FromServices]IGetAllUsersService service,
        [FromQuery]GetAllUsersRequest request)
    {
        JPaginatedResult<UserResult> result = null;
        await service.Create<IGetAllUsersService, GetAllUsersRequest, JPaginatedResult<UserResult>>()
            .AddFilter(request.xIsNotEmpty)
            .SetParameter(() => request)
            .SetValidator(new GetAllUsersRequest.Validator(Localizer))
            .OnValidated(v =>
                result = JPaginatedResult<UserResult>.Failure(v.Errors.Select(m => m.ErrorMessage).ToList()))
            .OnExecuted(r => result = r);
        
        return Ok(result);
    }
}