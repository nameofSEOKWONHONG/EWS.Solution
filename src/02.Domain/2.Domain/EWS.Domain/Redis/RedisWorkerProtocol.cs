using EWS.Domain.Common;
using EWS.Domain.Common.Enums;

namespace EWS.Domain.Example;

public record RedisWorkerProtocol
{
    public string TenantId { get; set; }
    public string Ip { get; set; }
    public string MachineName { get; set; }
    /// <summary>
    /// <see cref="https://stackoverflow.com/questions/55093538/check-what-environment-azure-functions-is-running-on"/>
    /// </summary>
    public string InstanceName { get; set; }
    public ENUM_REDIS_SUBSCRIBER_TYPE WorkerType { get; set; }
    /// <summary>
    /// REDIS에 SET 키에 대한 정보
    /// </summary>
    public string RedisKey { get; set; }    
    /// <summary>
    /// 특정 잡을 수행하는데 필요한 데이터 (JSON format)
    /// </summary>
    public string Request { get; set; }
    /// <summary>
    /// 작업 명령자 정보
    /// </summary>
    public string SenderConnectionId { get; set; }
    
    /// <summary>
    /// 작업 종료 후 수신자 정보
    /// </summary>
    public string Reciver { get; set; }

    public RedisWorkerProtocol(string tenantId, ENUM_REDIS_SUBSCRIBER_TYPE workerType, string redisKey)
    {
        this.TenantId = tenantId;
        this.WorkerType = workerType;
        this.RedisKey = redisKey;
    }
}