namespace Producer;

using Producer.Entities;
using Producer.Producers;
using RabbitMQ.Client;

public class ProducerFactory
{
    private readonly ConnectionFactory _factory;
    public ProducerFactory()
    {
        _factory = new ConnectionFactory { HostName = "localhost" };
    }

    public IProducer Create(ProducerSettings settings)
    {
        var channel = CreateChannel(settings);
        return new FanoutProducer(
            channel, 
            settings);
    }

    private IModel CreateChannel(ProducerSettings producerSettings)
    {
        if(producerSettings.TypeOfExchange == TypeOfExchange.Unknown || 
            !Enum.IsDefined(producerSettings.TypeOfExchange))
        {
            throw new ArgumentException($"Wrong TypeOfExchange '{producerSettings.TypeOfExchange}'", nameof(producerSettings));
        }

        if(producerSettings.TypeOfExchange == TypeOfExchange.FanOut)
            return CreateChannelForFanOut(producerSettings);

        if (producerSettings.TypeOfExchange == TypeOfExchange.Direct)
            return CreateChannelForDirect(producerSettings);

        throw new NotImplementedException($"Method not implemented for '{producerSettings.TypeOfExchange}'");
    }

    private IModel CreateChannelForDirect (ProducerSettings producerSettings)
    {
        var returnValue = _factory
            .CreateConnection()
            .CreateModel();

        returnValue.ExchangeDeclare(
            producerSettings.ExchangeName, 
            ExchangeType.Direct, 
            true, 
            true);

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
