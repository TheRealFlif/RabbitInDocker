using Producer.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer.Consumers;

internal class SimpleConsumer : ConsumerBase
{
    private static int _totalMessageCount = 0;
    private int messageCount = 0;

    internal SimpleConsumer(IModel channel, ConsumerSettings consumerSettings) : base(channel)
    {
        Name = consumerSettings.Name;
    }

    internal override void Consumer_Received(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        var deliveryTag = basicDeliverEventArgs.DeliveryTag;

        var body = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray());
        var message = GetMessage(Name, ++messageCount, body);

        Console.WriteLine(message);
        Channel.BasicAck(deliveryTag, false);

        base.Consumer_Received(sender, basicDeliverEventArgs);
    }

    private static object _lock = new();
    private static string GetMessage(string name, int localCount, string message)
    {
        lock (_lock)
        {
            _totalMessageCount++;
            return $"{name} (#{localCount:00} of {_totalMessageCount:00}): handling {message}";
        }
    }
}
