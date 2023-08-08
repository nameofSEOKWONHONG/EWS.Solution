using EWS.Entity;
using eXtensionSharp;

namespace EWS.Domain.Infra.Extension;

public class SecurityStamp
{
    public static string CreateSecurityStamp(User user)
    {
        if (user.xIsEmpty()) throw new Exception("user is empty.");
        return $"{user.Id}{user.FirstName}{user.LastName}{user.Email}{DateTime.UtcNow}".xGetHashCode();
    }
}