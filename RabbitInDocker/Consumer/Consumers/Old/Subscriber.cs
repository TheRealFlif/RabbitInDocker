//using RabbitMQ.Client.Events;
//using RabbitMQ.Client;
//using System.Text;
//using Consumer.Entities;

//namespace Consumer.Consumers;

//internal class Subscriber : IConsumer
//{
//    readonly IModel _channel;
//    readonly EventingBasicConsumer? _consumer;
//    readonly string _name;
//    readonly WaitTimeCreator _waitTimeCreator;

//    public event EventHandler? ExitMessageReceived;

//    public Subscriber(
//        IModel channel,
//        string name,
//        WaitTimeCreator waitTimeCreator)
//    {
//        _waitTimeCreator = waitTimeCreator;
//        _name = name;
//        _channel = channel;

//        try
//        {
//            _consumer = new EventingBasicConsumer(_channel);
//            _name ??= _consumer.ConsumerTags.FirstOrDefault(string.Empty);
//            _consumer.Received += Consumer_Received;
//            _channel.BasicConsume(_channel.CurrentQueue, false, _consumer);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"ooops: {ex.Message}");
//        }
//    }
        
//    protected virtual void Consumer_Received(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
//    {
//        var deliveryTag = basicDeliverEventArgs.DeliveryTag;
//        var body = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray());
//        var envelope = Envelope<string>.From(body);
//        var message = GetMessage(body);

//        var workTime = _waitTimeCreator.GetMilliseconds();
//        Console.WriteLine(message);
//        Thread.Sleep(workTime);

//        _channel.BasicAck(deliveryTag, false);
//        if ((envelope?.Data??string.Empty).StartsWith('q'))
//        {
//            ExitMessageReceived?.Invoke(this, new EventArgs());
//        }
//    }

//    protected virtual string GetMessage(string message)
//    {
//        var returnValue = string.Empty;
//        Envelope<string>? messageObject = Envelope<string>.From(message);

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
//        _channel?.Dispose();
//        //throw new NotImplementedException();
//    }
//}
