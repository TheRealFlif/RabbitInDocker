namespace Producer;

using Producer.Entities;
using Producer.Producers;
using RabbitMQ.Client;

public class ProducerFactory
{
    public IProducer Create(ProducerSettings settings)
    {
        return new FanoutProducer(CreateChannel(settings), settings);
    }

    private IModel CreateChannel(ProducerSettings producerSettings)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        IModel? returnValue = connection.CreateModel();
        returnValue.QueueDeclare(producerSettings.RoutingKey, true, false, false, null);

        return returnValue;
    }
}
