using System.Reflection;

namespace EWS.Domain;

public class JPermissions
{
    public static List<string> GetRegisteredPermissions()
    {
        var permissions = new List<string>();
        foreach (var prop in typeof(JPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
                permissions.Add(propertyValue.ToString());
        }
        return permissions;
    }
}