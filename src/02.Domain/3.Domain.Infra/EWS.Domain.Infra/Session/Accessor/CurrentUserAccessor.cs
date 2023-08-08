using System.Security.Claims;
using EWS.Application.Const;
using EWS.Domain.Infra.Redis;
using EWS.Infrastructure.Session.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EWS.Domain.Infra.Session.Accessor;

public class CurrentUserAccessor : ICurrentUserAccessor
{
    public string TenantId { get; }
    public string UserId { get; }
    public string UserName { get; }
    public string RoleName { get; }
    public string TimeZone { get; }
    public List<KeyValuePair<string, string>> Claims { get; set; }
    
    public CurrentUserAccessor(IHttpContextAccessor accessor)
    {
        TenantId = accessor.HttpContext?.User?.FindFirst(ApplicationClaimTypeConstants.TenantId)?.Value;
        TimeZone = accessor.HttpContext?.User?.FindFirst(ApplicationClaimTypeConstants.TimeZone)?.Value;
        UserId = accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        UserName = accessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
        RoleName = accessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        Claims = accessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
    }

    public CurrentUserAccessor(string tenantId, string userId, string userName, string roleName, string timeZone)
    {
        this.TenantId = tenantId;
        this.UserId   = userId;
        this.UserName = userName;
        this.RoleName = roleName;
        this.TimeZone = timeZone;
    }
}