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
        _consumers.Add(factory.Create(new ConsumerSettings(100, 100, "letterbox", "Pineapple", 10)));

        var queueNames = new[] { "letterbox", "letterbox", "letterbox" };
        queueNames
            .Select(xx => new ConsumerSettings(100, 100, xx, string.Empty, 1))
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
