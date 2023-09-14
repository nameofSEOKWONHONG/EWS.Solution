using System.Collections.Generic;
using System.Security.Claims;
using EWS.Application.Service.Abstract;
using EWS.Infrastructure.ServiceRouter.Abstract;
using Microsoft.IdentityModel.Tokens;

namespace EWS.Domain.Abstraction.Account.Identity;

public interface IGenerateEncryptedToken : IServiceImplBase<IdentityGenerateEncryptedTokenRequest, string>, IScopeService
{
    
}

public class IdentityGenerateEncryptedTokenRequest
{
    public SigningCredentials SigningCredentials { get; set; }
    public IEnumerable<Claim> Claims { get; set; }
}
