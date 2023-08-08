using EWS.Application.Language;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EWS.Domain.Infra.Session;

/// <summary>
/// 인증 API 에서 사용하는 session context
/// </summary>
public class SessionContext : ISessionContext
{
    public string TenantId { get; set; }
    public bool IsDecrypt { get; set; }
    public string SessionId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }

    public ICurrentUserAccessor CurrentUserAccessor { get; set; }
    public ICurrentTimeAccessor CurrentTimeAccessor { get; set; }
    public IInfraAccessor InfraAccessor { get; set; }
    public IConfiguration Configuration { get; set; }
    public ILocalizer Localizer { get; set; }

    public SessionContext(
        ICurrentUserAccessor currentUserAccessor,
        ICurrentTimeAccessor currentTimeAccessor,
        IInfraAccessor infraAccessor,
        IConfiguration configuration,
        ILocalizer localizer,
        IHttpContextAccessor httpContextAccessor)
    {
        CurrentUserAccessor = currentUserAccessor;
        CurrentTimeAccessor = currentTimeAccessor;
        InfraAccessor = infraAccessor;
        Configuration = configuration;
        Localizer = localizer;
        
        #if DEBUG
        this.TenantId = "00000";
        this.IsDecrypt = false;
        this.SessionId = string.Empty;
        this.UserId = string.Empty;
        this.UserName = string.Empty;
        #endif
    }
}