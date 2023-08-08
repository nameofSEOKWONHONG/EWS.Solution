namespace EWS.BlazorWasm;

public class AppConstants
{
    /// <summary>
    /// 서버측 연결 정보
    /// </summary>
    public class ServerHost
    {
        public const string URL = "https://localhost:7201"; //domain
        public const string NOTIFICATION_HUB_NAME = "notificationHub";
        public const string HOST_NAME = "EWS.WebApi.Server"; //"EWS.BlazorServer"
    }
    
    public class BlazorHost
    {
        public const string HOST_NAME = "EWS.BlazorServer";
    }
    
    public class WasmHost
    {
        public const string CULTURE_NAME = "culture-info";
        public const string INDEXED_DB_NAME = "ewsdb";
        public const string INDEXED_DB_STORE_NAME = "data-store";
        public const string HOST_NAME = "EWS.WebApi.Server"; //"EWS.BlazorServer"
        public const string CLIENT_NAME = "EWS.BlazorWasm";
        public const string HTTP_CLIENT_NAME = "EWS.API";
        public const string URL = "https://localhost:7201"; //domain
        public const string NOTIFICATION_HUB_NAME = "notificationHub";        
    }
}