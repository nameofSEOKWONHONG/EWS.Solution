using EWS.Application.Language;
using EWS.Domain.Base;

namespace EWS.Domain.Account.Users;

public class GetAllUsersRequest : JRequestBase
{
    public string Email { get; set; }
    public string UserName { get; set; }

    public class Validator : JValidatorBase<GetAllUsersRequest>
    {
        public Validator(ILocalizer localizer) : base(localizer)
        {
            NotEmpty(m => m.Email);
            NotEmpty(m => m.UserName);
            Must(m => m.Email, "@");
            Email(m => m.Email);
        }
    }
}
