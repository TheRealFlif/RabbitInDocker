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
        var channel = _factory
            .CreateConnection()
            .CreateModel();
        channel.BasicQos(0, consumerSettings.PrefetchCount, false);
        channel.ExchangeDeclare(consumerSettings.ExchangeName, ExchangeType.Fanout, true);

        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(
            queueName, 
            consumerSettings.ExchangeName, 
            consumerSettings.RoutingKey);

        var name = string.IsNullOrEmpty(consumerSettings.Name)
            ? CreateConsumerName(consumerSettings.QueueName)
            : consumerSettings.Name;
        var waiter = new WaitTimeCreator(consumerSettings.MinWait, consumerSettings.MaxWait);
        var returnValue = new Subscriber(
            channel, 
            name, 
            waiter);
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
