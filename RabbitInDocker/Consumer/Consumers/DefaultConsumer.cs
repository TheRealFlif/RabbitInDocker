#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer.Consumers;

public class DefaultConsumer : IConsumer
{
    readonly IModel _channel;
    readonly EventingBasicConsumer _consumer;
    private static int _totalMessageCount;
    private int messageCount;
    readonly string _name;
    public event EventHandler ExitMessageReceived;

    public DefaultConsumer(IModel channel)
    {
        _channel = channel;

        try
        {
            _consumer = new EventingBasicConsumer(_channel);
            _name = _consumer.ConsumerTags.FirstOrDefault(string.Empty);
            _consumer.Received += Consumer_Received;
            _channel.BasicConsume(_channel.CurrentQueue, false, _consumer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ooops: {ex.Message}");
        }
    }

    public DefaultConsumer(IModel channel, string name) : this(channel)
    {
        _name = name;
    }

    private void Consumer_Received(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        var deliveryTag = basicDeliverEventArgs.DeliveryTag;

        var body = basicDeliverEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        Console.WriteLine(GetMessage(_name, ++messageCount, message)); ;
        _channel.BasicAck(deliveryTag, false);

        if (message.StartsWith('q'))
        {
            ExitMessageReceived?.Invoke(this, new EventArgs());
        }
    }

    static readonly object _lock = new();
    private static string GetMessage(string name, int localCount, string message)
    {
        lock (_lock)
        {
            _totalMessageCount++;
            return $"{name} (#{localCount:00} of {_totalMessageCount:00}): handling {message}";
        }
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
        }
    }
}
