//#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
//using Consumer.Entities;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Text;

//namespace Consumer.Consumers;

//public class MessageConsumer : IConsumer
//{
//    readonly IModel _channel;
//    readonly EventingBasicConsumer _consumer;
//    private static int _totalMessageCount;
//    private int messageCount;
//    readonly string _name;
    
//    public event EventHandler ExitMessageReceived;

//    public MessageConsumer(IModel channel)
//    {
//        _channel = channel;

//        try
//        {
//            _consumer = new EventingBasicConsumer(_channel);
//            _name = _consumer.ConsumerTags.FirstOrDefault(string.Empty);
//            _consumer.Received += Consumer_Received;
//            _channel.BasicConsume(_channel.CurrentQueue, false, _consumer);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"ooops: {ex.Message}");
//        }
//    }

//    public MessageConsumer(IModel channel, string name) : this(channel)
//    {
//        _name = name;
//    }

//    private void Consumer_Received(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
//    {
//        var deliveryTag = basicDeliverEventArgs.DeliveryTag;

//        var body = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray());
//        var message = GetMessage(_name, ++messageCount, body);

//        Console.WriteLine(message);
//        _channel.BasicAck(deliveryTag, false);

//        if (message.StartsWith('q'))
//        {
//            ExitMessageReceived?.Invoke(this, new EventArgs());
//        }
//    }

//    private static object _lock = new();
//    private static string GetMessage(string name, int localCount, string message)
//    {
//        var returnValue = string.Empty;
//        Envelope<string>? messageObject;
//        try
//        {
//            messageObject = Envelope<string>.From(message);
//            if ((messageObject?.Data.StartsWith('q')).GetValueOrDefault())
//            {
//                return "q";
//            }
//        }
//        catch (Exception e)
//        {
//            return e.Message;
//        }

//        if (messageObject != null)
//        {
//            lock (_lock)
//            {
//                _totalMessageCount++;
//                var sender = messageObject["sender"];
//                var messageNumber = messageObject["messageNumber"];
//                returnValue = $"{name} (#{localCount:00} of {_totalMessageCount:00}): handling {messageObject.Data} from {sender} no: {messageNumber}";
//            }
//        }

//        return returnValue;
//    }

//    public void Dispose()
//    {
//        Dispose(true);
//        GC.SuppressFinalize(this);
//    }

//    protected virtual void Dispose(bool disposing)
//    {
//        if (disposing)
//        {
//            try
//            {
//                _channel?.Close();
//            }
//            finally
//            {
//                _channel?.Dispose();
//            }
//        }
//    }
//}
