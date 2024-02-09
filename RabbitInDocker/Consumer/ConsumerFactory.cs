using Consumer.Consumers;
using RabbitMQ.Client;

namespace Consumer;

public class ConsumerFactory : IConsumerFactory
{
    readonly ConnectionFactory _factory;
    readonly Dictionary<string, int> _consumerCounter = [];

    public ConsumerFactory()
    {
        _factory = new ConnectionFactory { HostName = "localhost" };
    }

    public IConsumer Create(string queueName)
    {
        var connection = _factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(queueName, true, false, false, null);

        if(_consumerCounter.TryGetValue(queueName, out var counter))
        {
            _consumerCounter.Remove(queueName);
        }
        _consumerCounter.Add(queueName, ++counter);
        
        var returnValue = new MessageConsumer(channel, $"{queueName}_{counter}");
        return returnValue;
    }
}
