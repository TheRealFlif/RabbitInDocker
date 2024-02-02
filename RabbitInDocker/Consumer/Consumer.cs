using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer;

public class Consumer : IDisposable
{
    ConnectionFactory _factory;
    IConnection _connection;
    IModel _channel;
    EventingBasicConsumer _consumer;

    public Consumer()
    {
        _factory = new ConnectionFactory { HostName = "localhost" };

        try
        {
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("letterbox", true, false, false, null);

            _consumer = new EventingBasicConsumer(_channel);

            _consumer.Received += Consumer_Received;
            _channel.BasicConsume("letterbox", false, _consumer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ooops: {ex.Message}");
        }
    }

    private int messageCount;
    private void Consumer_Received(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        var body = basicDeliverEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($"Message #{++messageCount:000}: {message}");
        _channel.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
        }
        finally
        {
            _channel?.Dispose();
        }

        try
        {
            _connection?.Close();
        }
        finally
        {
            _connection?.Dispose();
        }
    }
}
