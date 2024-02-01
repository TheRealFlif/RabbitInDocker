﻿using RabbitMQ.Client;

namespace Producer;

public class Producer : IDisposable
{
    ConnectionFactory _factory;
    IConnection _connection;
    IModel _channel;

    public Producer()
    {
        try
        {
            _factory = new ConnectionFactory { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("letterbox", true, false, false, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ooops: {ex.Message}");
        }
    }

    public void SendMessage(string message)
    {
        var body = System.Text.Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish("", "letterbox", null, body);
    }

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
