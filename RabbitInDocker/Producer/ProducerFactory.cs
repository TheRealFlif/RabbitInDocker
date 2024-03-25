namespace Producer;

using Producer.Entities;
using Producer.Producers;
using RabbitMQ.Client;
using System.ComponentModel.Design;

public class ProducerFactory
{
    private readonly ConnectionFactory _factory;
    public ProducerFactory()
    {
        _factory = new ConnectionFactory { HostName = "localhost" };
    }

    public IEnumerable<IProducer> Create(ProducerSettings settings)
    {
        var channel = CreateChannel(settings);
        if (settings.TypeOfExchange == ProducerType.FanOut)
        {
            return new[]{ new FanoutProducer(
            channel,
            settings)};
        }

        if (settings.TypeOfExchange == ProducerType.SimpleProducer)
        {
            return new[]{
                new SimpleProducer(channel, settings),
                new SimpleProducer(channel, settings),
                new SimpleProducer(channel, settings),
                new SimpleProducer(channel, settings)
            };
        }

        throw new NotImplementedException($"Cannot create a producer of type '{settings.TypeOfExchange}'");
    }

    private IModel CreateChannel(ProducerSettings producerSettings)
    {
        if (producerSettings.TypeOfExchange == ProducerType.Unknown ||
            !Enum.IsDefined(producerSettings.TypeOfExchange))
        {
            throw new ArgumentException($"Wrong TypeOfExchange '{producerSettings.TypeOfExchange}'", nameof(producerSettings));
        }

        if (producerSettings.TypeOfExchange == ProducerType.FanOut)
            return CreateChannelForFanOut(producerSettings);

        if (producerSettings.TypeOfExchange == ProducerType.SimpleProducer)
            return CreateChannelForDirect(producerSettings);

        throw new NotImplementedException($"Method not implemented for '{producerSettings.TypeOfExchange}'");
    }

    private IModel CreateChannelForDirect(ProducerSettings producerSettings)
    {
        var returnValue = _factory
            .CreateConnection()
            .CreateModel();

        returnValue.ExchangeDeclare(
            producerSettings.ExchangeName,
            ExchangeType.Direct,
            true,
            true);

        var queueName = returnValue.QueueDeclare(durable: true, exclusive: false, autoDelete: false).QueueName;
        returnValue.QueueBind(queueName, producerSettings.ExchangeName, producerSettings.RoutingKey, null);

        return returnValue;
    }

    private IModel CreateChannelForFanOut(ProducerSettings producerSettings)
    {
        var returnValue = _factory
            .CreateConnection()
            .CreateModel();
        returnValue.ExchangeDeclare(
            producerSettings.ExchangeName,
            ExchangeType.Fanout,
            true,
            true);

        return returnValue;
    }
}
