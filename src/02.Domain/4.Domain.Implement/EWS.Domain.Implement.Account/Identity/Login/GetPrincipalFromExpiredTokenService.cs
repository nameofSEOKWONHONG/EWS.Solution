using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EWS.Application.Const;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EWS.Domain.Implement.Account.Identity;

public class GetPrincipalFromExpiredTokenService : ServiceImplBase<GetPrincipalFromExpiredTokenService, string, ClaimsPrincipal>, IGetPrincipalFromExpiredTokenService
{
    private readonly IConfiguration _configuration;
    public GetPrincipalFromExpiredTokenService(DbContext dbContext, IConfiguration configuration) : base(dbContext, null)
    {
        _configuration = configuration;
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override Task OnExecuteAsync()
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Application:Secret").xToSHA512Decrypt(ApplicationConstants.Encryption.DB_ENC_SHA512_KEY))),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(this.Request, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        this.Result = principal;

        return Task.CompletedTask;
    }
}