using RabbitMQ.Client;

namespace Producer;

public class Producer
{
    public void SendMessage(string message)
    {
        ConnectionFactory factory = new ConnectionFactory { HostName = "localhost" };
        IConnection connection = factory.CreateConnection();
        IModel channel = connection.CreateModel();

        channel.QueueDeclare("letterbox");
        var body = System.Text.Encoding.UTF8.GetBytes(message);

        channel.BasicPublish("", "letterbox", null, body);
    }

}
