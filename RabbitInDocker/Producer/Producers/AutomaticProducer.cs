#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Producer.Entities;
using RabbitMQ.Client;
using System.Text.Json;

namespace Producer.Producers;

/// <summary>
/// Creates messages automtically with a randomized interval
/// </summary>
public class AutomaticProducer : IProducer
{
    readonly ProducerSettings _producerSettings;

    public AutomaticProducer(ProducerSettings producerSettings)
    {
        _producerSettings = producerSettings;
    }

    /// <summary>
    /// Sends the message with a routing key
    /// </summary>
    /// <param name="message">Message to send</param>
    public void SendMessages(int numberOfMessages)
    {
        using var channel = CreateChannel();
        for (int i = 0; i < numberOfMessages; i++)
        {
            var messageObject = CreateNewMessage(Guid.NewGuid().ToString("N"));
            var message = JsonSerializer.Serialize(messageObject);
            var body = System.Text.Encoding.UTF8.GetBytes(message ?? string.Empty);

            Console.WriteLine($"{_producerSettings.Name} sending message: {messageObject.Data}");
            channel.BasicPublish("", _producerSettings.RoutingKey, null, body);
            Sleep();
        }
    }

    private IModel? CreateChannel()
    {
        IModel? returnValue = null;
        try
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            returnValue = connection.CreateModel();
            returnValue.QueueDeclare(_producerSettings.RoutingKey, true, false, false, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ooops: {ex.Message}");
        }

        return returnValue;
    }

    int _messageNumber = 0;
    private Envelope<string> CreateNewMessage(string body)
    {
        var returnValue = new Envelope<string>(body);
        returnValue["sender"] = _producerSettings.Name;
        returnValue["messageNumber"] = $"{++_messageNumber:00}";
        return returnValue;
    }

    static readonly Random _random = new();
    private void Sleep()
    {
        var sleepInMillisec = _random.Next(_producerSettings.MinWait, _producerSettings.MaxWait);
        Console.WriteLine($"{_producerSettings.Name} sleeping {sleepInMillisec} milliseconds");
        Thread.Sleep(sleepInMillisec);
    }

    public void ShutDown()
    {
        using var channel = CreateChannel();
        var shutDownMessage = CreateNewMessage("q");
        channel.BasicPublish("", _producerSettings.RoutingKey, null, System.Text.Encoding.UTF8.GetBytes(shutDownMessage.To()));
    }
}
