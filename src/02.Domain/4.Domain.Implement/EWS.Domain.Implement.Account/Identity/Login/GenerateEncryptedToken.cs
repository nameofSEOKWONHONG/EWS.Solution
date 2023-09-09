using System.IdentityModel.Tokens.Jwt;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Account.Identity;

public class GenerateEncryptedToken : ScopeServiceImpl<GenerateEncryptedToken, IdentityGenerateEncryptedTokenRequest, string>, IGenerateEncryptedToken
{
    public GenerateEncryptedToken(IHttpContextAccessor accessor) : base(accessor)
    {
    }

    public override Task<bool> OnExecutingAsync(DbContext dbContext, ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override Task OnExecuteAsync(DbContext dbContext, ISessionContext context)
    {
        var token = new JwtSecurityToken(
            claims: this.Request.Claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: this.Request.SigningCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        this.Result = encryptedToken;
        return Task.CompletedTask;
    }
}