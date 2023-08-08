namespace EWS.Application.Const;

public class ApplicationConstants
{
    public class Encryption
    {
        public const string DB_ENC_SHA512_KEY = "AKIEUDNXMZ8823@28S3!!";
        public const string DB_ENC_IV = "WXNDFLGSZFOQJKJK";
        public const string DB_ENC_KEY = "BLKQJFFFBJUQUBHIAICLMJFVZZNLTXII";
    }
    
    public class Redis
    {
        public const string MessageChannel = "EWS:REDIS:CHANNEL";
    }
    
    public class SignalR
    {
        public static string NotificationHubName = "notificationHub";
    }
    
    public class Limit
    {
        public const int ACCESS_LIMIT_COUNT = 5;
    }
}