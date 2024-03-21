using Consumer;
using Consumer.Consumers;
using Producer.Entities;

namespace ConsumerProgram;

internal class Program
{
    readonly static System.Collections.Concurrent.BlockingCollection<IConsumer> _consumers = [];

    static void Main(string[] args)
    {
        MainForDefaultConsumer();
        
        //MainForFanOut();
    }

    private static void MainForDefaultConsumer()
    {
        var factory = new ConsumerFactory(ExitMessageReceived);
        var settings = new ConsumerSettings(1, 1000, "letterbox", "DefaultConsumer", 1) { 
            ExchangeName = "Direct",
            ConsumerType = ConsumerType.MessageConsumer
        };
        var consumer = factory.Create(settings);
         while (true) ;
    }

    private static void MainForFanOut()
    {
        var factory = new ConsumerFactory(ExitMessageReceived);
        var loggerSettings = ConsumerSettings.SubscriberSettings("Logger", "Pubsub", "");

        var consumers = new System.Collections.Concurrent.BlockingCollection<IConsumer>();
        consumers.Add(factory.Create(loggerSettings));

        var queueNames = new[] { "letterbox1", "letterbox2", "letterbox3" };
        queueNames
            .Select(n => ConsumerSettings.SubscriberSettings(n, "Pubsub", string.Empty))
            .ToList()
            .ForEach(cs => consumers.Add(factory.Create(cs)));

        var running = true;
        while (running) ;
    }

    private static void ExitMessageReceived(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }
}
