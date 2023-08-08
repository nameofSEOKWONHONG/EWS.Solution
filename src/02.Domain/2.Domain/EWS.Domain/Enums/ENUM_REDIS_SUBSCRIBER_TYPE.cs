using Ardalis.SmartEnum;

namespace EWS.Domain.Common.Enums;

// public enum ENUM_REDIS_SUBSCRIBER_TYPE
// {
//     EXPORT,
//     END
// }

public sealed class ENUM_REDIS_SUBSCRIBER_TYPE : SmartEnum<ENUM_REDIS_SUBSCRIBER_TYPE>
{
    public static readonly ENUM_REDIS_SUBSCRIBER_TYPE Export = new ENUM_REDIS_SUBSCRIBER_TYPE(nameof(Export), 0);
    public static readonly ENUM_REDIS_SUBSCRIBER_TYPE End = new ENUM_REDIS_SUBSCRIBER_TYPE(nameof(End), 1);
    
    public ENUM_REDIS_SUBSCRIBER_TYPE(string name, int value) : base(name, value)
    {
    }
}