namespace EWS.Domain.Infra.Kafka;

public class ApacheKafkaOption
{
    public string Topic { get; set; }
    public string GroupId { get; set; }
    public string BootstrapServers { get; set; }
}