using EWS.Application.Language;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.Extensions.Configuration;

namespace EWS.Domain.Infra.Session;

/// <summary>
/// 비인증 API에 사용되는 session context (비인증시 사용)
/// </summary>
public class UnverifiedContext : ISessionContext
{
    public string TenantId { get; set; }
    public bool IsDecrypt { get; set; } = false;
    public string SessionId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }

    /// <summary>
    /// not working http request
    /// </summary>
    public ICurrentUserAccessor CurrentUserAccessor { get; set; }

    public ICurrentTimeAccessor CurrentTimeAccessor { get; set; }

    public IInfraAccessor InfraAccessor { get; set; }
    public IConfiguration Configuration { get; set; }
    public ILocalizer Localizer { get; set; }

    public UnverifiedContext()
    {
        
    }
}