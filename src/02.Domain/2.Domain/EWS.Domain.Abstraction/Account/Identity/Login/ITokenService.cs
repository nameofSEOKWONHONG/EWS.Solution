using EWS.Application.Service.Abstract;
using EWS.Application.Wrapper;
using EWS.Domain.Identity;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Account.Identity;

public interface ITokenService : IServiceImplBase<TokenRequest, IResultBase<TokenResponse>>, IScopeService
{
    
}