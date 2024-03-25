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
        var returnValue = CreateConsumers(consumerSettings);

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

    private IEnumerable<IConsumer> CreateConsumers(ConsumerSettings consumerSettings)
    {
        if(ConsumerType.SimpleConsumer == consumerSettings.ConsumerType)
        {
            return CreateDirectConsumers(1, consumerSettings);
        }

        if(ConsumerType.SleepingConsumer == consumerSettings.ConsumerType)
        {
            return CreateSleepingConsumers(consumerSettings);
        }

        if(ConsumerType.Subscriber == consumerSettings.ConsumerType)
        {
            return CreateSubscriberConsumers(4, consumerSettings);
        }

        throw new ArgumentException(
            $"Unable to create consumer of type '{consumerSettings.ConsumerType}'", 
            nameof(consumerSettings));
    }

    private static IEnumerable<IConsumer> CreateDirectConsumers(int numberOfConsumers, ConsumerSettings consumerSettings)
    {
        var returnValue = new List<IConsumer>();
        for(var i = 0; i<numberOfConsumers; i++)
        {
            returnValue.Add(new SimpleConsumer(CreateChannelForDirect(consumerSettings), consumerSettings));
        }

        return returnValue;
    }

    private static IEnumerable<IConsumer> CreateSleepingConsumers(ConsumerSettings consumerSettings)
    {
        var returnValue = new List<IConsumer>();

        consumerSettings.MinWait = 2000;
        consumerSettings.MaxWait = 2000;
        returnValue.Add(new SleepingConsumer(CreateChannelForDirectWithQoS(consumerSettings, 8000), consumerSettings));

        consumerSettings.MinWait = 4000;
        consumerSettings.MaxWait = 4000;
        returnValue.Add(new SleepingConsumer(CreateChannelForDirectWithQoS(consumerSettings, 8000), consumerSettings));

        consumerSettings.MinWait = 8000;
        consumerSettings.MaxWait = 8000;
        returnValue.Add(new SleepingConsumer(CreateChannelForDirectWithQoS(consumerSettings, 8000), consumerSettings));

        return returnValue;
    }

    private static IEnumerable<IConsumer> CreateSubscriberConsumers(int numberOfConsumers, ConsumerSettings consumerSettings)
    {
        var queueNames = new[] { "Promotion", "UserHandling", "Audit", "Payment" };
        var returnValue = new List<IConsumer>();
        for (var i = 0; i < numberOfConsumers; i++)
        {
            var name = queueNames[i % queueNames.Length];
            var channel = CreateChannelForSubscriber(consumerSettings, name);
            returnValue.Add(new SimpleConsumer(channel, consumerSettings, name));
        }

        return returnValue;
    }

    private static IModel CreateChannelForDirect(ConsumerSettings consumerSettings)
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

    private static IModel CreateChannelForDirectWithQoS(ConsumerSettings consumerSettings, int maxWaitTime)
    {
        var returnValue = Connection.CreateModel();

        var result = returnValue.QueueDeclare(
            queue: consumerSettings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var prefetch = maxWaitTime/consumerSettings.MaxWait;
        returnValue.BasicQos(0, (ushort)prefetch, false); //remove this line to use round robin

        returnValue.QueueBind(
            result.QueueName,
            consumerSettings.ExchangeName,
            consumerSettings.RoutingKey);

        return returnValue;
    }

    private static IModel CreateChannelForSubscriber(ConsumerSettings consumerSettings, string name)
    {
        var returnValue = Connection.CreateModel();

        var result = returnValue.QueueDeclare(
            queue: name,
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
