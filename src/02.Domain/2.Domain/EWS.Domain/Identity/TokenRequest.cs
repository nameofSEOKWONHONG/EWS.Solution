using EWS.Application.Language;
using EWS.Domain.Base;
using FluentValidation;

namespace EWS.Domain.Identity;

public class TokenRequest : JRequestBase
{
    public string TenantId { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
    
    public bool RememberMe { get; set; }
    
    public string ConnectionId { get; set; }
    
    public class Validator : JValidatorBase<TokenRequest>
    {
        public Validator(ILocalizer localizer) : base(localizer)
        {
            RuleFor(m => m.TenantId)
                .NotEmpty()
                ;
            //.WithMessage(localizer[""].xValue(""));

            RuleFor(m => m.Email)
                .NotEmpty();

            RuleFor(m => m.Password)
                .NotEmpty();
        }
    }
}