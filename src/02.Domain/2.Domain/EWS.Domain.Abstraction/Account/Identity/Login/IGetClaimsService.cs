using System.Collections.Generic;
using System.Security.Claims;
using EWS.Entity;
using EWS.Infrastructure.ServiceRouter.Abstract;

namespace EWS.Domain.Abstraction.Account.Identity;

public interface IGetClaimsService: IServiceImplBase<User, IEnumerable<Claim>>
{
    
}
