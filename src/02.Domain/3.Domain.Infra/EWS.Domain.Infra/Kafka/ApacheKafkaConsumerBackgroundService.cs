using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EWS.Domain.Infra.Kafka;

public class ApacheKafkaConsumerBackgroundService : BackgroundService
{
    private Serilog.ILogger _logger => Serilog.Log.Logger;
    private ApacheKafkaOption _apacheKafkaOption;
    private ApacheKafkaConsumerCallback _callback;
    
    public ApacheKafkaConsumerBackgroundService(IOptionsMonitor<ApacheKafkaOption> options,
        ApacheKafkaConsumerCallback callback)
    {
        _apacheKafkaOption = options.CurrentValue;
        _callback = callback;
        options.OnChange((config) =>
        {
            _apacheKafkaOption = config;
        });
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig()
        {
            GroupId = _apacheKafkaOption.GroupId,
            BootstrapServers = _apacheKafkaOption.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        try
        {
            using var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build();
            consumerBuilder.Subscribe(_apacheKafkaOption.Topic);
            
            _logger.Information("Consuming messages from topic: {Topic}", _apacheKafkaOption.Topic);
            
            try
            {
                LoopStart:
                var consumer = consumerBuilder.Consume(stoppingToken);
                var orderRequest = JsonSerializer.Deserialize<OrderProcessingRequest>(consumer.Message.Value);
                _logger.Information("Processing Order Id: {Id}", orderRequest.OrderId);
                    
                _callback.Callback(orderRequest);
                goto LoopStart;
            }
            catch (OperationCanceledException)
            {   
                consumerBuilder.Close();
            }
        }
        catch (Exception e)
        {
            _logger.Error(e, "Kafka Consumer Error : {Error}", e.Message);
            throw;
        }

        return Task.CompletedTask;
    }
}