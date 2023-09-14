﻿using System.Security.Claims;
using EWS.Domain.Abstraction.Account.Identity;
using EWS.Domain.Infrastructure;
using EWS.Entity;
using EWS.Infrastructure.ServiceRouter.Abstract;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EWS.Domain.Implement.Account.Identity;

public class GenerateJwtTokenService: ServiceImplBase<GenerateJwtTokenService, User, string>, IGenerateJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IGenerateEncryptedTokenService _generateEncryptedTokenService;
    private readonly IGetClaimsService _getClaimsService;
    private readonly IGetSigningCredentialsService _getSigningCredentialsService;
    public GenerateJwtTokenService(IConfiguration configuration, 
        IGenerateEncryptedTokenService generateEncryptedTokenService,
        IGetClaimsService getClaimsService,
        IGetSigningCredentialsService getSigningCredentialsService) : base()
    {
        _configuration = configuration;
        _generateEncryptedTokenService = generateEncryptedTokenService;
        _getClaimsService = getClaimsService;
        _getSigningCredentialsService = getSigningCredentialsService;
    }
    
    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override async Task OnExecuteAsync()
    {   
        SigningCredentials signingCredentials = null;
        IEnumerable<Claim> claims = null;
        await ServiceLoader<IGetClaimsService, User, IEnumerable<Claim>>.Create(_getClaimsService)
            .AddFilter(() => this.Request.xIsNotEmpty())
            .SetParameter(() => this.Request)
            .OnExecuted((res, v) => claims = res);
        
        await ServiceLoader<IGetSigningCredentialsService, bool, SigningCredentials>.Create(_getSigningCredentialsService)
            .AddFilter(() => claims.xIsNotEmpty())
            .OnExecuted((res, v) => signingCredentials = res);
        
        await ServiceLoader<IGenerateEncryptedTokenService, IdentityGenerateEncryptedTokenRequest, string>.Create(_generateEncryptedTokenService)
            .AddFilter(() => claims.xIsNotEmpty())
            .AddFilter(() => signingCredentials.xIsNotEmpty())
            .SetParameter(() => new IdentityGenerateEncryptedTokenRequest()
            {
                SigningCredentials = signingCredentials,
                Claims = claims
            })
            .OnExecuted((res, v) =>
            {
                this.Result = res;
            });
    }
}
