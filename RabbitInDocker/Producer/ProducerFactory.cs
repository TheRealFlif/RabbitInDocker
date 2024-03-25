namespace Producer;

using Producer.Entities;
using Producer.Producers;
using RabbitMQ.Client;
using System.ComponentModel.Design;

public class ProducerFactory
{
    private static readonly ConnectionFactory _factory = new ConnectionFactory { HostName = "localhost" };
    private static IConnection _connection;

    public IEnumerable<IProducer> Create(ProducerSettings settings)

    {
        if (new[] { ProducerType.SimpleProducer, ProducerType.FanOut }.Contains(settings.TypeOfExchange))
        {
            return CreateProducers(1, settings);
        }

        throw new NotImplementedException($"Cannot create a producer of type '{settings.TypeOfExchange}'");
    }

    private IEnumerable<IProducer> CreateProducers(int numberOfProducers, ProducerSettings settings)
    {
        var returnValue = new List<IProducer>();
        for (int i = 0; i < numberOfProducers; i++)
        {
            var channel = CreateChannel(settings);
            returnValue.Add(new SimpleProducer(channel, settings));
        }
        return returnValue;
    }

    private IModel CreateChannel(ProducerSettings producerSettings)
    {
        if (producerSettings.TypeOfExchange == ProducerType.SimpleProducer)
            return CreateChannelForDirect(producerSettings);

        if (producerSettings.TypeOfExchange == ProducerType.FanOut)
            return CreateChannelForFanOut(producerSettings);

        throw new NotImplementedException($"Method not implemented for '{producerSettings.TypeOfExchange}'");
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
                    if (_connection == null)
                    {
                        _connection = _factory.CreateConnection();
                    }
                }
            }

            return _connection;
        }

    }

    private IModel CreateChannelForDirect(ProducerSettings producerSettings)
    {
        var returnValue = Connection.CreateModel();

        returnValue.ExchangeDeclare(
            producerSettings.ExchangeName,
            ExchangeType.Direct,
            true,
            true);

        //var queueName = returnValue.QueueDeclare(durable: true, exclusive: false, autoDelete: false).QueueName;
        //returnValue.QueueBind(queueName, producerSettings.ExchangeName, producerSettings.RoutingKey, null);

        return returnValue;
    }

    private IModel CreateChannelForDirectWithQoS(ProducerSettings producerSettings)
    {
        var returnValue = Connection.CreateModel();

        returnValue.ExchangeDeclare(
            producerSettings.ExchangeName,
            ExchangeType.Direct,
            true,
            true);

        //var queueName = returnValue.QueueDeclare(durable: true, exclusive: false, autoDelete: false).QueueName;
        //returnValue.QueueBind(queueName, producerSettings.ExchangeName, producerSettings.RoutingKey, null);

        return returnValue;
    }

    private IModel CreateChannelForFanOut(ProducerSettings producerSettings)
    {
        var returnValue = Connection.CreateModel();
        returnValue.ExchangeDeclare(
            producerSettings.ExchangeName,
            ExchangeType.Fanout,
            true,
            true);

        return returnValue;
    }
}
