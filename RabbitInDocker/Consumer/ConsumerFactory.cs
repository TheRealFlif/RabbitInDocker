using Consumer.Consumers;
using Producer.Entities;
using RabbitMQ.Client;

namespace Consumer;

public class ConsumerFactory : IConsumerFactory
{
    private readonly static ConnectionFactory _factory = new ConnectionFactory { HostName = "localhost" };
    private static IConnection _connection;

    private readonly EventHandler _exitMessageReceived;

    public ConsumerFactory(EventHandler exitMessageReceived)
    {
        _exitMessageReceived = exitMessageReceived;
    }

    public IEnumerable<IConsumer> Create(ConsumerSettings consumerSettings)
    {
        var returnValue = CreateConsumer(consumerSettings);

        foreach(var consumer in returnValue)
        {
            consumer.ExitMessageReceived += _exitMessageReceived;
        }
        
        return returnValue;
    }

    private void ConsumerFactory_ExitMessageReceived(object? sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private IEnumerable<IConsumer> CreateConsumer(ConsumerSettings consumerSettings)
    {
        if(new[] { ConsumerType.Default, ConsumerType.Lazy, ConsumerType.MessageConsumer }.Contains(consumerSettings.ConsumerType))
        {
            var channel = CreateChannelForDirect(consumerSettings);
            return new[] {
                new SimpleConsumer(channel, consumerSettings),
                //new SimpleConsumer(channel, consumerSettings),
                //new SimpleConsumer(channel, consumerSettings),
                //new SimpleConsumer(channel, consumerSettings)
            };

        }

        throw new ArgumentException(
            $"Unable to create consumer of type '{consumerSettings.ConsumerType}'", 
            nameof(consumerSettings));
    }

    private IModel CreateChannelForDirect(ConsumerSettings consumerSettings)
    {
        var returnValue = Connection.CreateModel();
        
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

    private static object _lock = new();
    private static IConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                lock(_lock)
                {
                    if(_connection == null)
                    {
                        _connection = _factory.CreateConnection();
                    }
                }
            }

            return _connection;
        }
        
    }
}
