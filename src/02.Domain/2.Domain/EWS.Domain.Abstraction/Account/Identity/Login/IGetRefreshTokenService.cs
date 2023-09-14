using EWS.Application.Service.Abstract;
using EWS.Entity;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Account.Identity;

public interface IGetRefreshTokenService : IServiceImplBase<User, string>, IScopeService
{
    
    
}