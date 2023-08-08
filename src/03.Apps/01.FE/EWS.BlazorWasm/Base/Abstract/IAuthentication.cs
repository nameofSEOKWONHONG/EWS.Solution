using System.Security.Claims;
using EWS.Application.Wrapper;
using EWS.Domain.Identity;

namespace EWS.BlazorWasm.Base;

public interface IAuthentication
{
    Task<IResultBase> Login(TokenRequest model);

    Task<IResultBase> Logout();

    Task<string> RefreshToken();

    Task<string> TryRefreshToken();

    Task<string> TryForceRefreshToken();

    Task<IWasmSessionContext> CurrentUser();
}