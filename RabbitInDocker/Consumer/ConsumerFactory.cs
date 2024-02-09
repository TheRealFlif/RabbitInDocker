using Consumer.Consumers;
using RabbitMQ.Client;

namespace Consumer;

public class ConsumerFactory : IConsumerFactory
{
    readonly ConnectionFactory _factory;
    readonly Dictionary<string, int> _consumerCounter = [];

    readonly int _minSleep;
    readonly int _maxSleep;

    public ConsumerFactory() : this(1000, 4000) { }

    public ConsumerFactory(int minSleep, int maxSleep)
    {
        _minSleep = minSleep < maxSleep ? minSleep : maxSleep;
        _maxSleep = maxSleep > minSleep ? maxSleep : minSleep;

        _factory = new ConnectionFactory { HostName = "localhost" };
    }

    public IConsumer Create(string queueName)
    {
        return Create(queueName, CreateConsumerName(queueName));
    }

    public IConsumer Create(string queueName, string consumerName)
    {
        var connection = _factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(queueName, true, false, false, null);

        var returnValue = new LazyConsumer(channel, consumerName, _minSleep, _maxSleep);
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
