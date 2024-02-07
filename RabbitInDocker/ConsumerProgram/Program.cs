using Consumer;

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
            if (consumer is DefaultConsumer defaultConsumer)
            {
                defaultConsumer.ExitMessageReceived += ExitMessageReceived;
            }

            if (consumer is MessageConsumer messageConsumer)
            {
                messageConsumer.ExitMessageReceived += ExitMessageReceived;
            }
        }

        var running = true;
        while (running) ;
    }

    private static void ExitMessageReceived(object? sender, EventArgs e)
    {
        foreach (var consumer in _consumers)
        {
            consumer?.Dispose();
        }
        Environment.Exit(0);
    }
}
