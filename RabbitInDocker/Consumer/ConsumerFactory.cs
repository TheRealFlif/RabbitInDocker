using Consumer.Consumers;
using Producer.Entities;
using RabbitMQ.Client;

namespace Consumer;

public class ConsumerFactory : IConsumerFactory
{
    readonly ConnectionFactory _factory;
    readonly Dictionary<string, int> _consumerCounter = [];

    private EventHandler _exitMessageReceived;
    public ConsumerFactory(EventHandler exitMessageReceived)
    {
        _exitMessageReceived = exitMessageReceived;
        _factory = new ConnectionFactory { HostName = "localhost" };
    }

    public IConsumer Create (ConsumerSettings consumerSettings)
    {
        var connection = _factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(consumerSettings.QueueName, true, false, false, null);
        channel.BasicQos(0, consumerSettings.PrefetchCount, false);

        string name = string.IsNullOrEmpty(consumerSettings.Name)
            ? CreateConsumerName(consumerSettings.QueueName)
            : consumerSettings.Name;
        var returnValue = new LazyConsumer(channel, name, consumerSettings.MinWait, consumerSettings.MaxWait);
        returnValue.ExitMessageReceived += _exitMessageReceived;
        
        return returnValue;
    }

    private string CreateConsumerName(string queueName)
    {
        if (_consumerCounter.TryGetValue(queueName, out var counter))
        {
            _consumerCounter.Remove(queueName);
        }
        _consumerCounter.Add(queueName, ++counter);

        return $"{queueName}_{counter}";
    }
}
