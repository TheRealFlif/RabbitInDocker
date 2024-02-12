using Consumer;
using Consumer.Consumers;

namespace ConsumerProgram;

internal class Program
{
    readonly static System.Collections.Concurrent.BlockingCollection<IConsumer> _consumers = [];

    static void Main(string[] args)
    {
        var factory = new ConsumerFactory();
        _consumers.Add(factory.Create("letterbox"));
        _consumers.Add(factory.Create("letterbox"));
        _consumers.Add(factory.Create("letterbox"));
        _consumers.Add(factory.Create("letterbox"));

        foreach (var consumer in _consumers)
        {
            if (consumer is IConsumer iConsumer)
            {
                iConsumer.ExitMessageReceived += ExitMessageReceived;
            }
        }

        var running = true;
        while (running) ;
    }

    private static void ExitMessageReceived(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }
}
