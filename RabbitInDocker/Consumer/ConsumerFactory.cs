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

    public IConsumer Create(ConsumerSettings? consumerSettings)
    {
        if (consumerSettings == null) { throw new ArgumentNullException(nameof(consumerSettings)); }

        var connection = _factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.BasicQos(0, consumerSettings?.PrefetchCount ?? 1, false);
        channel.ExchangeDeclare(consumerSettings?.ExchangeName, ExchangeType.Fanout, true);

        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queueName, consumerSettings?.ExchangeName, consumerSettings?.RoutingKey);

        string? name = string.IsNullOrEmpty(consumerSettings?.Name)
            ? CreateConsumerName(consumerSettings?.QueueName)
            : consumerSettings?.Name;
        var returnValue = new Subscriber(channel, name ?? string.Empty, new WaitTimeCreator(consumerSettings?.MinWait ?? 0, consumerSettings?.MaxWait ?? 0));
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
