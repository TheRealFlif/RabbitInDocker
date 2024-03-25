using Consumer.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer.Consumers;

internal abstract class ConsumerBase : IConsumer
{
    protected readonly IModel Channel;
    protected readonly EventingBasicConsumer Consumer;
    public string Name { get; protected set; }
    public string QueueName { get; protected set; }
    public event EventHandler ExitMessageReceived;

    internal ConsumerBase(IModel channel)
    {
        Channel = channel;

        try
        {
            Consumer = new EventingBasicConsumer(Channel);
            QueueName = Channel.CurrentQueue;
            Consumer.Received += Consumer_Received;
            Channel.BasicConsume(QueueName, false, Consumer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ooops: {ex.Message}");
        }
    }

    internal virtual void Consumer_Received(object? sender, BasicDeliverEventArgs basicDeliverEventArgs)
    {
        var json = Encoding.UTF8.GetString(basicDeliverEventArgs.Body.ToArray());
        var envelope = Envelope<string>.From(json) ?? new Envelope<string>("q");
        if (string.Compare(envelope.Data, "q", StringComparison.InvariantCultureIgnoreCase) == 0)
        {
            ExitMessageReceived?.Invoke(this, new EventArgs());
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
