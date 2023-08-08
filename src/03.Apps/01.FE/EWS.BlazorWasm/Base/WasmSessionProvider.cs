#nullable enable
using System.Security.Claims;
using EWS.Application.Const;
using EWS.BlazorWasm.Setup;

namespace EWS.BlazorWasm.Base;

public class WasmSessionProvider
{
    public string? TenantId { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? MobilePhone { get; set; }
    public string? UserId { get; set; }
    public string? Role { get; set; }
    

    private readonly JAuthenticationStateProvider _stateProvider;
    
    public WasmSessionProvider(JAuthenticationStateProvider stateProvider)
    {
        _stateProvider = stateProvider;
    }

    public async Task<IWasmSessionContext> GetSessionAsync()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        return new WasmSessionContext()
        {
            TenantId = state.User.GetTenantId(),
            Email = state.User.GetEmail(),
            UserName = state.User.GetUserName(),
            MobilePhone = state.User.GetPhoneNumber(),
            UserId = state.User.GetUserId(),
            Role = state.User.GetRole()
        };
    }
}

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;

    /// <summary>
    /// 사용자명
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static string? GetUserName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;

    public static string? GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirst(ClaimTypes.MobilePhone)?.Value;

    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public static string? GetRole(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;

    public static string? GetTenantId(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirst(ApplicationClaimTypes.TenantId)?.Value;
}