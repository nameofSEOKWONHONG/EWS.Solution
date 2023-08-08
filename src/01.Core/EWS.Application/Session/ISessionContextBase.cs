namespace EWS.Application.Session;

public interface ISessionContextBase
{
    string TenantId { get; set; }
    bool IsDecrypt { get; set; }
    string SessionId { get; set; }
    string UserId { get; set; }
    string UserName { get; set; }
}