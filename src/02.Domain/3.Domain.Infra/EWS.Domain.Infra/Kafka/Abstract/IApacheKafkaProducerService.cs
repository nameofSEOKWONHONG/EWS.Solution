namespace EWS.Domain.Infra.Kafka.Abstract;

public interface IApacheKafkaProducerService
{
    Task<bool> Send<T>(string bootstrapServers, string topic, T sendItem)
        where T : class;
} 