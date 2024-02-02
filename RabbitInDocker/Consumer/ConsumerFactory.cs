using RabbitMQ.Client;

namespace Consumer;

public class ConsumerFactory : IConsumerFactory
{
    ConnectionFactory _factory;

    public ConsumerFactory()
    {
        _factory = new ConnectionFactory { HostName = "localhost" };
    }

    public IConsumer Create(string queueName)
    {
        var connection = _factory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(queueName, true, false, false, null);

        return new DefaultConsumer(channel);
    }
}
