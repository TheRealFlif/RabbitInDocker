using Consumer.Entities;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Consumer.Consumers;

public class LazyConsumer : IConsumer
{
    readonly IModel _channel;
    readonly EventingBasicConsumer? _consumer;
    private static int _totalMessageCount;
    private int messageCount;
    readonly string _name;
    public event EventHandler? ExitMessageReceived;

    private int _minWait;
    private int _maxWait;

    public LazyConsumer(IModel channel, string name) : this(channel, name, 100, 100)
    {

    }

    public LazyConsumer(IModel channel, string name, int minWait, int maxWait)
    {
        _minWait = minWait < maxWait ? minWait : maxWait;
        _maxWait = maxWait > minWait ? maxWait : minWait;
        _name = name;
        _channel = channel;

        try
        {
            _consumer = new EventingBasicConsumer(_channel);
            _name = _name ?? _consumer.ConsumerTags.FirstOrDefault(string.Empty);
            _consumer.Received += Consumer_Received;
            _channel.BasicConsume(_channel.CurrentQueue, false, _consumer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ooops: {ex.Message}");
        }
    }

    private static Random _random = new Random();

    private void Consumer_Received(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        var workTime = _random.Next(_minWait, _maxWait);
        Console.WriteLine($"{_name} received message and working for {workTime} ms.");
        var deliveryTag = basicDeliverEventArgs.DeliveryTag;

        var body = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray());
        var message = GetMessage(_name, ++messageCount, body);

        Thread.Sleep(workTime);
        Console.WriteLine(message);
        _channel.BasicAck(deliveryTag, false);

        if (message.StartsWith('q'))
        {
            ExitMessageReceived?.Invoke(this, new EventArgs());
        }
    }

    private static object _lock = new();
    private static string GetMessage(string name, int localCount, string message)
    {
        var returnValue = string.Empty;
        Envelope<string>? messageObject;
        try
        {
            messageObject = Envelope<string>.From(message);
            if ((messageObject?.Data.StartsWith('q')).GetValueOrDefault())
            {
                returnValue = "q";
            }
        }
        catch (Exception e)
        {
            return e.Message;
        }
        if (messageObject != null)
        {
            lock (_lock)
            {
                _totalMessageCount++;
                var sender = messageObject["sender"];
                var messageNumber = messageObject["messageNumber"];

                return $"{name} (#{localCount:00} of {_totalMessageCount:00}): handling {messageObject.Data} from {sender} no: {messageNumber}";
            }
        }

        return returnValue;
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
