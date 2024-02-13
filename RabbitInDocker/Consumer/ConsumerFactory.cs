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

    private static bool _first = true;
    public IConsumer Create(string queueName, string consumerName)
    {
        var connection = _factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(queueName, true, false, false, null);
        var prefetchSize = 0u; // the size of message buffer in bytes that the client can use to prefetch messages, 0 = no limit and is the only allowed value in RabbitMQ.Client
        ushort prefetchCount = (ushort)(_first ? 10 : 1); // the number of messages to retrieve before stop sending new messages to the channel
        _first = false;
        channel.BasicQos(prefetchSize, prefetchCount, false); //false = setttings apply only to this channel and consumers on the channel
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
