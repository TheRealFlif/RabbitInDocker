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
        var returnValue = CreateFanoutConsumer(consumerSettings);

        returnValue.ExitMessageReceived += _exitMessageReceived;

        return returnValue;
    }

    private IConsumer CreateFanoutConsumer(ConsumerSettings consumerSettings)
    {
        var channel = CreateChannelForFanOut(consumerSettings);

        var name = string.IsNullOrEmpty(consumerSettings.Name)
            ? CreateConsumerName(consumerSettings.QueueName)
            : consumerSettings.Name;
        var waiter = new WaitTimeCreator(consumerSettings.MinWait, consumerSettings.MaxWait);
        var returnValue = new Subscriber(
            channel,
            name,
            waiter);
        
        return returnValue;
    }

    private IModel CreateChannelForFanOut(ConsumerSettings consumerSettings)
    {
        var returnValue = _factory
            .CreateConnection()
            .CreateModel();
        returnValue.BasicQos(0, consumerSettings.PrefetchCount, false);
        returnValue.ExchangeDeclare(consumerSettings.ExchangeName, ExchangeType.Fanout, true);

        var queueName = returnValue.QueueDeclare().QueueName;
        returnValue.QueueBind(
            queueName,
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
