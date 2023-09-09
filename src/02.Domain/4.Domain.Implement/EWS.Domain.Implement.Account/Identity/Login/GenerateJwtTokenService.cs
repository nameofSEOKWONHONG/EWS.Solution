using System.Security.Claims;
using System.Transactions;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using EWS.Infrastructure.ServiceRouter.Implement.Routers;
using EWS.Infrastructure.Session.Abstract;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EWS.Domain.Implement.Account.Identity;

public class GenerateJwtTokenService: ScopeServiceImpl<GenerateJwtTokenService, User, string>, IGenerateJwtTokenService
{
    private readonly IConfiguration _configuration;
    public GenerateJwtTokenService(IHttpContextAccessor accessor) : base(accessor)
    {
        _configuration = accessor.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
    }

    public override Task<bool> OnExecutingAsync(ISessionContext context)
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync(ISessionContext context)
    {
        SigningCredentials signingCredentials = null;
        IEnumerable<Claim> claims = null;
        using (var sr = ServiceRouter.Create<EWSMsDbContext>(this.Accessor))
        {
            sr.Register<IGetClaimsService, User, IEnumerable<Claim>>()
                .AddFilter(() => this.Request.xIsNotEmpty())
                .SetParameter(() => this.Request)
                .Executed(res => claims = res);
            
            sr.Register<IGetSigningCredentialsService, bool, SigningCredentials>()
                .AddFilter(() => true)
                .SetParameter(() => true)
                .Executed(res => signingCredentials = res);
            
            sr.Register<IGenerateEncryptedToken, IdentityGenerateEncryptedTokenRequest, string>()
                .AddFilter(() => claims.xIsNotEmpty())
                .AddFilter(() => signingCredentials.xIsNotEmpty())
                .SetParameter(() => new IdentityGenerateEncryptedTokenRequest()
                {
                    SigningCredentials = signingCredentials,
                    Claims = claims
                })
                .Executed(res => this.Result = res);
            
            await sr.ExecuteAsync();
        }
    }
}