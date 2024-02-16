using Consumer;
using Consumer.Consumers;
using Producer.Entities;

namespace ConsumerProgram;

internal class Program
{
    readonly static System.Collections.Concurrent.BlockingCollection<IConsumer> _consumers = [];

    static void Main(string[] args)
    {
        var factory = new ConsumerFactory(ExitMessageReceived);
        var loggerSettings = ConsumerSettings.SubscriberSettings("Logger", "Pubsub", "");
         
        _consumers.Add(factory.Create(loggerSettings));

        var queueNames = new[] { "letterbox", "letterbox", "letterbox" };
        queueNames
            .Select(xx => ConsumerSettings.SubscriberSettings(xx, "Pubsub", string.Empty))
            .ToList()
            .ForEach(cs => _consumers.Add(factory.Create(cs)));

        var running = true;
        while (running) ;
    }

    private static void ExitMessageReceived(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }
}
