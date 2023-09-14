using EWS.Application.Service.Abstract;
using EWS.Application.Wrapper;
using EWS.Domain.Identity;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Account.Identity;

public interface ITokenRefreshService : IServiceImplBase<RefreshTokenRequest, IResultBase<TokenResponse>>, IScopeService
{
    
}