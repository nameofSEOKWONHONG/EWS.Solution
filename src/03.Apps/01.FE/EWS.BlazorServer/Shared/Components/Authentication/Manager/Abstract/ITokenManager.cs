using EWS.Application.Wrapper;
using EWS.BlazorShared.Base;
using EWS.Domain.Identity;

namespace EWS.BlazorServer.Shared.Authentication.Manager;

public interface ITokenManager : IManager
{
    Task<IResultBase> Login(TokenRequest model);
}