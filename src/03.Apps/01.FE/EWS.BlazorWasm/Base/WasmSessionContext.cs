#nullable enable
namespace EWS.BlazorWasm.Base;

public class WasmSessionContext : IWasmSessionContext
{
    public string? TenantId { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? MobilePhone { get; set; }
    public string? UserId { get; set; }
    public string? Role { get; set; }
}