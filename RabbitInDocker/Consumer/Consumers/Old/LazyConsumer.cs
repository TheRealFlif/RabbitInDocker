//using Consumer.Entities;
//using RabbitMQ.Client.Events;
//using RabbitMQ.Client;
//using System.Text;

//namespace Consumer.Consumers;

//public class LazyConsumer : IConsumer
//{
//    readonly IModel _channel;
//    readonly EventingBasicConsumer? _consumer;
//    readonly string _name;
//    readonly WaitTimeCreator _waitTimeCreator;
    
//    public event EventHandler? ExitMessageReceived;

//    public LazyConsumer(IModel channel, string name, WaitTimeCreator waitTimeCreator)
//    {
//        _waitTimeCreator = waitTimeCreator;
//        _name = name;
//        _channel = channel;

//        try
//        {
//            _consumer = new EventingBasicConsumer(_channel);
//            _name = _name ?? _consumer.ConsumerTags.FirstOrDefault(string.Empty);
//            _consumer.Received += Consumer_Received;
//            _channel.BasicConsume(_channel.CurrentQueue, false, _consumer);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"ooops: {ex.Message}");
//        }
//    }

//    private static Random _random = new Random();

//    private void Consumer_Received(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
//    {
//        var deliveryTag = basicDeliverEventArgs.DeliveryTag;
//        var body = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray());
//        var message = GetMessage(body);

//        if (message == "q")
//        {
//            Console.WriteLine($"{_name} got shut down message");
//            _channel.BasicAck(deliveryTag, false);
//            Thread.Sleep(500);
//            ExitMessageReceived?.Invoke(this, new EventArgs());
//            return;
//        }
//        else
//        {
//            Console.WriteLine(message);
//            Thread.Sleep(_waitTimeCreator.GetMilliseconds());
//        }

//        _channel.BasicAck(deliveryTag, false);
//    }

//    private string GetMessage(string message)
//    {
//        var returnValue = string.Empty;
//        Envelope<string>? messageObject;
//        try
//        {
//            messageObject = Envelope<string>.From(message);
//            if ((messageObject?.Data.StartsWith('q')).GetValueOrDefault())
//            {
//                returnValue = "q";
//            }
//        }
//        catch (Exception e)
//        {
//            return e.Message;
//        }
//        if (messageObject != null && string.IsNullOrEmpty(returnValue))
//        {
//            var sender = messageObject["sender"];
//            var messageNumber = messageObject["messageNumber"];

//            returnValue = $"{_name} got message {sender}:{messageNumber}";
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
