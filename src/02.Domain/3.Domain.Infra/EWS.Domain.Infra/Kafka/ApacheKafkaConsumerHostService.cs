using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace EWS.Domain.Infra.Kafka;

public class ApacheKafkaConsumerHostService : IHostedService
{
    private readonly Serilog.ILogger _logger;
    private ApacheKafkaOption _apacheKafkaOption;
    public ApacheKafkaConsumerHostService(ILogger logger, IOptionsMonitor<ApacheKafkaOption> options)
    {
        _logger = logger;
        _apacheKafkaOption = options.CurrentValue;
        options.OnChange((config) =>
        {
            _apacheKafkaOption = config;
        });
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
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
            var cancelToken = new CancellationTokenSource();

            try
            {
                while (true)
                {
                    var consumer = consumerBuilder.Consume(cancelToken.Token);
                    var orderRequest = JsonSerializer.Deserialize<OrderProcessingRequest>(consumer.Message.Value);
                    _logger.Debug("Processing Order Id: {Id}", orderRequest.OrderId);
                }
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

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}