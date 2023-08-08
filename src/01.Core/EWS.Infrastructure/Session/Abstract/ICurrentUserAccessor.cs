namespace EWS.Infrastructure.Session.Abstract;

public interface ICurrentUserAccessor
{
    string TenantId { get; }
    string UserId { get; }
    string UserName { get; }
    string RoleName { get; }
    string TimeZone { get; }
    List<KeyValuePair<string, string>> Claims { get; set; }
}