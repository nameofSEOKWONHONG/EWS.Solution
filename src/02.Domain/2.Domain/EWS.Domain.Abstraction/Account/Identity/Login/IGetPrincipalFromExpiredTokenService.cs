using System.Security.Claims;
using EWS.Application.Service.Abstract;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Account.Identity;

public interface IGetPrincipalFromExpiredTokenService : IServiceImplBase<string, ClaimsPrincipal>, IScopeService
{
    
}