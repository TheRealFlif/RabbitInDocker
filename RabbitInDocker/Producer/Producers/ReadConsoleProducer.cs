#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Producer.Entities;
using RabbitMQ.Client;

namespace Producer.Producers;

public class ReadConsoleProducer : ProducerBase<string>
{
    /// <summary>
    /// 
    /// </summary>
    public ReadConsoleProducer(IModel channel, ProducerSettings settings) :base(channel, settings) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message">Message to send</param>
    public void SendMessage(string? message)
    {
        var body = System.Text.Encoding.UTF8.GetBytes(message ?? string.Empty);
        Channel.BasicPublish(Settings.ExchangeName, Settings.RoutingKey, null, body);
    }
}
