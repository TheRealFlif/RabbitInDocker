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
            var channel = CreateChannelForDirect(consumerSettings);
            return new[] {
                new SimpleConsumer(CreateChannelForDirect(consumerSettings), consumerSettings),
                //new SimpleConsumer(CreateChannelForDirect(consumerSettings), consumerSettings),
                //new SimpleConsumer(CreateChannelForDirect(consumerSettings), consumerSettings),
                //new SimpleConsumer(CreateChannelForDirect(consumerSettings), consumerSettings)
            };
        }

        if(ConsumerType.SleepingConsumer == consumerSettings.ConsumerType)
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

    private IModel CreateChannelForDirectWithQoS(ConsumerSettings consumerSettings, int maxWaitTime)
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
