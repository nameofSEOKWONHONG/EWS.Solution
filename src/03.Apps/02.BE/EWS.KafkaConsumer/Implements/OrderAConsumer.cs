using EWS.Domain.Infra.Kafka;
using EWS.KafkaConsumer.Abstract;

namespace EWS.KafkaConsumer.Implements;

public class OrderAConsumer : IKafkaConsumer
{
    public Task ExecuteAsync(OrderProcessingRequest request)
    {
        throw new NotImplementedException();
    }
}
