#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.Common;
using System.Text;

namespace Consumer;

public class DefaultConsumer : IConsumer
{
    readonly IModel _channel;
    readonly EventingBasicConsumer _consumer;
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
        
        if(message.StartsWith('q'))
        {
            Console.WriteLine($"{_name} (#{++messageCount:000}): handling {message}");
            _channel.BasicAck(deliveryTag, false); //Om vi inte ackar meddeleandet så ligger den kvar på kön och stänger ner konsument-programmet
            ExitMessageReceived?.Invoke(this, new EventArgs());    
        }

        if (message.StartsWith('t'))
        {
            Console.WriteLine($"{_name} (#{++messageCount:000}): handling {message}");
            _channel.BasicAck(deliveryTag, false);
        }
        else
        {
            Console.WriteLine($"{_name} (#{++messageCount:000}): unable handling {message}");
            var requeue = !basicDeliverEventArgs.Redelivered;
            _channel.BasicNack(deliveryTag, false, requeue);
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
