﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using RabbitMQ.Client;
using System.Text.Json;

namespace Producer.Producers;

/// <summary>
/// Creates messages automtically with a randomized interval
/// </summary>
public class AutomaticProducer : IDisposable
{
    readonly ConnectionFactory _factory;
    readonly IConnection _connection;
    readonly IModel _channel;
    readonly int _minWait;
    readonly int _maxWait;
    readonly string _name;

    public AutomaticProducer(int minWait, int maxWait, string queueName) : this(minWait, maxWait, queueName, $"{queueName}_{Guid.NewGuid():N}")
    {

    }

    /// <summary>
    /// Creates a producer that sends a message between minWait and maxWait milliseconds to a queue 
    /// </summary>
    public AutomaticProducer(int minWait, int maxWait, string queueName, string name)
    {
        _minWait = minWait < maxWait ? minWait : maxWait;
        _maxWait = maxWait > minWait ? maxWait : minWait;
        _name = name;

        try
        {
            _factory = new ConnectionFactory { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queueName, true, false, false, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ooops: {ex.Message}");
        }
    }

    static readonly Random _random = new();
    /// <summary>
    /// Sends the message with a routing key = "letterbox"
    /// </summary>
    /// <param name="message">Message to send</param>
    public void SendMessages(int numberOfMessages)
    {
        for (int i = 0; i < numberOfMessages; i++)
        {
            var data = Guid.NewGuid().ToString("N");
            var messageObject = new Entities.Envelope<string>(data);
            messageObject["sender"] = _name;
            messageObject["messageNumber"] = $"{i + 1:00}";


            var message = JsonSerializer.Serialize(messageObject);
            var body = System.Text.Encoding.UTF8.GetBytes(message ?? string.Empty);

            Console.WriteLine($"{_name} sending message: {data}");
            _channel.BasicPublish("", "letterbox", null, body);

            var sleepInMillisec = _random.Next(_minWait, _maxWait);
            Console.WriteLine($"{_name} sleeping {sleepInMillisec} milliseconds");
            Thread.Sleep(sleepInMillisec);
        }
    }

    public void ShutDown()
    {
        var shutDownMessage = new Entities.Envelope<string>("q");
        _channel.BasicPublish("", "letterbox", null, System.Text.Encoding.UTF8.GetBytes(shutDownMessage.To()));
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

            try
            {
                _connection?.Close();
            }
            finally
            {
                _connection?.Dispose();
            }
        }
    }
}