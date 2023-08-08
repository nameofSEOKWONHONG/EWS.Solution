using EWS.Infrastructure.ServiceRouter.Abstract;
using Microsoft.IdentityModel.Tokens;

namespace EWS.Domain.Abstraction.Account.Identity;

public interface IGetSigningCredentialsService : IServiceImplBase<bool, SigningCredentials>
{
    
}