namespace EWS.Domain.Identity;

public class RefreshTokenRequest
{
    public string TenantId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}