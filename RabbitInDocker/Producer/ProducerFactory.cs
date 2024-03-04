namespace Producer;

using Producer.Entities;
using Producer.Producers;
using RabbitMQ.Client;

public class ProducerFactory
{
    public IProducer Create(ProducerSettings settings)
    {
        var channel = CreateChannel(settings);
        return new FanoutProducer(
            channel, 
            settings);
    }

    private static IModel CreateChannel(ProducerSettings producerSettings)
    {
        if(producerSettings.TypeOfExchange == TypeOfExchange.Unknown || 
            !Enum.IsDefined(producerSettings.TypeOfExchange))
        {
            throw new ArgumentException(nameof(producerSettings));
        }

        if(producerSettings.TypeOfExchange == TypeOfExchange.FanOut)
            return CreateChannelForFanOut(producerSettings);

        throw new NotImplementedException($"Method not implemented for '{producerSettings.TypeOfExchange}'");
    }

    private static IModel CreateChannelForFanOut(ProducerSettings producerSettings)
    {
        var returnValue = new ConnectionFactory { HostName = "localhost" }
            .CreateConnection()
            .CreateModel();
        returnValue.ExchangeDeclare(
            producerSettings.ExchangeName, 
            ExchangeType.Fanout, 
            true, 
            false);

        return returnValue;
    }
}
