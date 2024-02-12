#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using RabbitMQ.Client;

namespace Producer.Producers;

public class ReadConsoleProducer : Entities.IProducer
{
    readonly ConnectionFactory _factory;
    readonly IConnection _connection;
    readonly IModel _channel;

    /// <summary>
    /// Creates a producer that sends a message to a channel named "letterbox"
    /// </summary>
    public ReadConsoleProducer()
    {
        try
        {
            _factory = new ConnectionFactory { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("letterbox", true, false, false, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ooops: {ex.Message}");
        }
    }

    /// <summary>
    /// Sends the message with a routing key = "letterbox"
    /// </summary>
    /// <param name="message">Message to send</param>
    public void SendMessage(string? message)
    {
        var body = System.Text.Encoding.UTF8.GetBytes(message ?? string.Empty);
        _channel.BasicPublish("", "letterbox", null, body);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
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
}
