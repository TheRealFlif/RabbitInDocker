using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer;

public class DefaultConsumer : IConsumer
{
    IModel _channel;
    EventingBasicConsumer _consumer;

    public DefaultConsumer(IModel channel)
    {
        try
        {
            _channel = channel;
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += Consumer_Received;
            _channel.BasicConsume(_channel.CurrentQueue, false, _consumer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ooops: {ex.Message}");
        }
    }

    private int messageCount;
    private void Consumer_Received(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        var deliveryTag = basicDeliverEventArgs.DeliveryTag;
        
        var body = basicDeliverEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        
        if(message.StartsWith("q"))
        {
            Console.WriteLine($"Handling message #{++messageCount:000}: {message}");
            _channel.BasicAck(deliveryTag, false); //Om vi inte ackar meddeleandet så ligger den kvar på kön och stänger ner konsument-programmet
            ExitMessageReceived.Invoke(this, new EventArgs());    
        }

        if (message.StartsWith("t"))
        {
            Console.WriteLine($"Handling message #{++messageCount:000}: {message}");
            _channel.BasicAck(deliveryTag, false);
        }
        else
        {
            Console.WriteLine($"Unable to handle message #{++messageCount:000}: {message}");
            var requeue = !basicDeliverEventArgs.Redelivered;
            _channel.BasicNack(deliveryTag, false, requeue);
        }
    }
    public event EventHandler ExitMessageReceived;
    

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
    }
}
