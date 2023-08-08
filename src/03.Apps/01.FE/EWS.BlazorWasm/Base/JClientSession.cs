using EWS.Application.Session;

namespace EWS.BlazorWasm.Base;

public class JClientSession : ISessionContextBase
{
    public string TenantId { get; set; }
    public bool IsDecrypt { get; set; }
    public string SessionId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
}