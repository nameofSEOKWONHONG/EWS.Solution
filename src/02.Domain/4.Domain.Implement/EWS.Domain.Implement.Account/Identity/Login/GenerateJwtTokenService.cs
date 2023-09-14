using System.Security.Claims;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Infrastructure;
using EWS.Entity;
using EWS.Entity.Db;
using EWS.Infrastructure.ServiceRouter.Abstract;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EWS.Domain.Implement.Account.Identity;

public class GenerateJwtTokenService: ServiceImplBase<GenerateJwtTokenService, User, string>, IGenerateJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IGenerateEncryptedToken _service;
    public GenerateJwtTokenService(IConfiguration configuration, IGenerateEncryptedToken service) : base()
    {
        _configuration = configuration;
        _service = service;
    }
    
    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {
        var result = string.Empty;
        await ServiceLoader<IGenerateEncryptedToken, IdentityGenerateEncryptedTokenRequest, string>.Create(_service)
            .AddFilter(() => true)
            .AddFilter(() => true)
            .SetParameter(() => new IdentityGenerateEncryptedTokenRequest())
            .OnExecuted((res) =>
            {
                result = res;
                return Task.CompletedTask;
            });
        
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