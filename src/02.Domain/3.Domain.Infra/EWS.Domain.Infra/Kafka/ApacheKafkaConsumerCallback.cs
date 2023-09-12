namespace EWS.Domain.Infra.Kafka;

public class ApacheKafkaConsumerCallback
{
    public Action<OrderProcessingRequest> Callback { get; set; }
    public ApacheKafkaConsumerCallback()
    {
        
    }
}