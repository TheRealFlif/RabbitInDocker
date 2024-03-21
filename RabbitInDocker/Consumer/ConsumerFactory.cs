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
        var name = string.IsNullOrEmpty(consumerSettings.Name)
            ? CreateConsumerName(consumerSettings.QueueName)
            : consumerSettings.Name;
        var waiter = new WaitTimeCreator(consumerSettings.MinWait, consumerSettings.MaxWait);
        
        if (consumerSettings.ConsumerType == ConsumerType.Subscriber)
        {
            var channel = CreateChannelForFanOut(consumerSettings);
            return new Subscriber(
                channel,
                name,
                waiter);
        }
        if(new[] { ConsumerType.Default, ConsumerType.Lazy, ConsumerType.MessageConsumer }.Contains(consumerSettings.ConsumerType))
        {
            var channel = CreateChannelForDirect(consumerSettings);
            if (consumerSettings.ConsumerType == ConsumerType.Default)
                return new DefaultConsumer(channel);
            if (consumerSettings.ConsumerType == ConsumerType.Lazy)
                return new LazyConsumer(channel, name, waiter);
            if (consumerSettings.ConsumerType == ConsumerType.MessageConsumer)
                return new MessageConsumer(channel);
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
        returnValue.BasicQos(0, consumerSettings.PrefetchCount, false);
        returnValue.ExchangeDeclare(
            consumerSettings.ExchangeName,
            ExchangeType.Direct,
            true,
            true);
        
        var result =  returnValue.QueueDeclare(consumerSettings.QueueName, true, false, autoDelete:true);
        var queueName = result.QueueName;
        returnValue.QueueBind(
            queueName,
            consumerSettings.ExchangeName,
            consumerSettings.RoutingKey);

        return returnValue;
    }

    private IModel CreateChannelForFanOut(ConsumerSettings consumerSettings)
    {
        var returnValue = _factory
            .CreateConnection()
            .CreateModel();
        returnValue.BasicQos(0, consumerSettings.PrefetchCount, false);
        returnValue.ExchangeDeclare(
            consumerSettings.ExchangeName, 
            ExchangeType.Fanout, 
            true, 
            true);

        var queueName = returnValue.QueueDeclare(exclusive: false).QueueName;
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
