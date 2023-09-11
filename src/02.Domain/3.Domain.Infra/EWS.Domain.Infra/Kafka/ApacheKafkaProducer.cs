using System.Net;
using Confluent.Kafka;
using EWS.Domain.Infra.Kafka.Abstract;
using eXtensionSharp;
using Serilog;

namespace EWS.Domain.Infra.Kafka;

public class ApacheKafkaProducerService : IApacheKafkaProducerService
{
    private Serilog.ILogger _logger => Serilog.Log.Logger;
    public ApacheKafkaProducerService()
    {
        
    }

    public async Task<bool> ProduceAsync<T>(string bootstrapServers, string topic, T sendItem)
    where T : class
    {
        var config = new ProducerConfig()
        {
            BootstrapServers = bootstrapServers,
            ClientId = Dns.GetHostName()
        };

        try
        {
            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var result = await producer.ProduceAsync(topic, new Message<Null, string>() { Value = sendItem.xToJson() });
            _logger.Debug("Delivery Timestamp {DateTime}", result.Timestamp.UtcDateTime);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            _logger.Error(e, "ApacheKafka Producer Error : {Error}", e.Message);
        }
        return await Task.FromResult(false);
    }
    
}