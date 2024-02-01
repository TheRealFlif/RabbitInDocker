using RabbitMQ.Client;

namespace Producer;

public class Producer
{
    ConnectionFactory _factory;

    public Producer()
    {
        _factory = new ConnectionFactory { HostName = "localhost" };
    }

    public void SendMessage(string message)
    {
        try
        {
            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare("letterbox", true, false, false, null);
                    var body = System.Text.Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish("", "letterbox", null, body);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ooops: {ex.Message}");
        }
    }

}
