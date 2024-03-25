using Consumer.Consumers;
using Producer.Entities;
using RabbitMQ.Client;

namespace Consumer;

public class ConsumerFactory : IConsumerFactory
{
    readonly ConnectionFactory _factory;
    readonly Dictionary<string, int> _consumerCounter = [];

    readonly EventHandler _exitMessageReceived;

    public ConsumerFactory(EventHandler exitMessageReceived)
    {
        _exitMessageReceived = exitMessageReceived;
        _factory = new ConnectionFactory { HostName = "localhost" };
    }

    public IConsumer Create(ConsumerSettings consumerSettings)
    {
        var returnValue = CreateConsumer(consumerSettings);

        returnValue.ExitMessageReceived += _exitMessageReceived;

        return returnValue;
    }

    private IConsumer CreateConsumer(ConsumerSettings consumerSettings)
    {
        if(new[] { ConsumerType.Default, ConsumerType.Lazy, ConsumerType.MessageConsumer }.Contains(consumerSettings.ConsumerType))
        {
            var channel = CreateChannelForDirect(consumerSettings);
            return new SimpleConsumer(channel, consumerSettings);
        }

        throw new ArgumentException(
            $"Unable to create consumer of type '{consumerSettings.ConsumerType}'", 
            nameof(consumerSettings));
    }

    private IModel CreateChannelForDirect(ConsumerSettings consumerSettings)
    {
        var returnValue = _factory
            .CreateConnection()
            .CreateModel();
        
        var result =  returnValue.QueueDeclare(
            queue: consumerSettings.QueueName, 
            durable: true, 
            exclusive: false, 
            autoDelete: true);

        returnValue.QueueBind(
            result.QueueName,
            consumerSettings.ExchangeName,
            consumerSettings.RoutingKey);

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
