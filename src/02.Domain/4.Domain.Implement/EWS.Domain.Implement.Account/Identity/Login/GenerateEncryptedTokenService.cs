using System.IdentityModel.Tokens.Jwt;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EWS.Domain.Implement.Account.Identity;

public class GenerateEncryptedTokenService : ServiceImplBase<GenerateEncryptedTokenService, IdentityGenerateEncryptedTokenRequest, string>, IGenerateEncryptedTokenService
{
    public GenerateEncryptedTokenService() : base()
    {
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override Task OnExecuteAsync()
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