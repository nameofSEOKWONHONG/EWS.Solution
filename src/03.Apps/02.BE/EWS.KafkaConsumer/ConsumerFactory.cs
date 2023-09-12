using Confluent.Kafka;
using EWS.Domain.Infra.Kafka;
using EWS.KafkaConsumer.Abstract;
using EWS.KafkaConsumer.Implements;
using eXtensionSharp;

namespace EWS.KafkaConsumer;

public class ConsumerFactory
{
    private readonly Dictionary<string, Func<IKafkaConsumer>> _states
        = new()
        {
            {
                "A", () => new OrderAConsumer()
            }
        };


    private IKafkaConsumer _selectedConsumer;
    
    private ConsumerFactory(string name)
    {
        _selectedConsumer = _states[name]();
    }

    public static ConsumerFactory Create(string name)
    {
        return new ConsumerFactory(name);
    }

    public Task ExecuteAsync(OrderProcessingRequest request)
    {
        if (_selectedConsumer.xIsEmpty()) throw new NotImplementedException();
        return _selectedConsumer.ExecuteAsync(request);
    }
}