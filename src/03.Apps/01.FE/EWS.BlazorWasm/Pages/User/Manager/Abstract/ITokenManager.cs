using EWS.Application.Wrapper;
using EWS.BlazorWasm.Base;
using EWS.Domain.Identity;

namespace EWS.BlazorWasm.Pages.User.Manager;

public interface ITokenManager : IManager
{
    Task<IResultBase> Login(TokenRequest model);
}