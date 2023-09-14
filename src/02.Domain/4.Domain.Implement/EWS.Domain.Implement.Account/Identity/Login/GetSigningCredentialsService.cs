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

public class GetSigningCredentialsService : ServiceImplBase<GetSigningCredentialsService, bool, SigningCredentials>, IGetSigningCredentialsService
{
    private readonly IConfiguration _configuration;
    public GetSigningCredentialsService(DbContext dbContext, IConfiguration configuration) : base(dbContext, null)
    {
        _configuration = configuration;
    }

    public override Task<bool> OnExecutingAsync()
    {
        return Task.FromResult(true);
    }

    public override Task OnExecuteAsync()
    {
        var secret = Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Application:Secret").xToSHA512Decrypt(ApplicationConstants.Encryption.DB_ENC_SHA512_KEY));
        this.Result = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        return Task.CompletedTask;
    }
}