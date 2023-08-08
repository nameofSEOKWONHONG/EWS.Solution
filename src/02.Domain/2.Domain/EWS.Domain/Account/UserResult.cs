using EWS.Domain.Base;

namespace EWS.Domain.Account;

public class UserResult : JDisplayRow
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirm { get; set; }
    public string PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public int AccessFailedCount { get; set; }
}