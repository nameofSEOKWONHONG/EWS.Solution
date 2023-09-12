using EWS.Domain.Infra.Kafka;

namespace EWS.KafkaConsumer.Abstract;

public interface IKafkaConsumer
{
    Task ExecuteAsync(OrderProcessingRequest request);
}