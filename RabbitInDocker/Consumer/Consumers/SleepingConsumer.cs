using Producer.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer.Consumers;

internal class SleepingConsumer : ConsumerBase
{
    private WaitTimeCreator _waitTimeCreator;

    internal SleepingConsumer(IModel channel, ConsumerSettings consumerSettings) : base(channel)
    {
        Name = Guid.NewGuid().ToString("N");
        _waitTimeCreator = new WaitTimeCreator(consumerSettings.MinWait, consumerSettings.MaxWait);
    }

    internal override void Consumer_Received(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        var deliveryTag = basicDeliverEventArgs.DeliveryTag;

        var body = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray());
        var sleepTime = _waitTimeCreator.GetMilliseconds();
        var message = GetMessage(Name, body, sleepTime);
        Console.WriteLine(message);
        Thread.Sleep(sleepTime);
        Channel.BasicAck(deliveryTag, false); //Onödig om det är auto-ack på kanalen

        base.Consumer_Received(sender, basicDeliverEventArgs);
    }

    private static string GetMessage(string name, string message, int milliseconds)
    {
        return $"{name.AsSpan(0, 4)}...: handling {message} and sleeping for {milliseconds}";
    }
}
