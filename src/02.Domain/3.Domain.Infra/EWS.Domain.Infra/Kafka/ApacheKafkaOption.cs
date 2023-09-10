namespace EWS.Domain.Infra.Kafka;

public class ApacheKafkaOption
{
    public string Topic { get; set; }= "test";
    public string GroupId { get; set; }= "test_group";
    public string BootstrapServers { get; set; }= "localhost:9092";
}