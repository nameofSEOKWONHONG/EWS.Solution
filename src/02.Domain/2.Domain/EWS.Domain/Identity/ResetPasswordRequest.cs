namespace EWS.Domain.Identity;

public class ResetPasswordRequest
{
    public string Email { get; set; }
    /// <summary>
    /// 원본 암호
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// 수정할 암호
    /// </summary>
    public string ConfirmPassword { get; set; }
    public string Token { get; set; }
}