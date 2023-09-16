using EWS.Application.Language;
using EWS.Domain.Base;

namespace EWS.Domain.Identity;

public class RefreshTokenRequest
{
    public string TenantId { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    
    public class Valdiator : JValidatorBase<RefreshTokenRequest>
    {
        public Valdiator(ILocalizer localizer) : base(localizer)
        {
            NotEmpty(m => m.Token);
            NotEmpty(m => m.RefreshToken);
        }
    }
}