using EWS.Entity;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Account.Identity;

public interface IGenerateJwtTokenService : IServiceImplBase<User, string>
{
    
}