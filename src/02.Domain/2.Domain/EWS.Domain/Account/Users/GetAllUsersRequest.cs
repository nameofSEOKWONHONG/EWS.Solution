using EWS.Domain.Base;

namespace EWS.Domain.Account.Users;

public class GetAllUsersRequest : JRequestBase
{
    public string Email { get; set; }
    public string UserName { get; set; }
}
