namespace ProductionProgram;

using Producer;
using Producer.Entities;

internal class Program
{
    static ProducerFactory factory = new ProducerFactory();
    static ConsoleReader _consoleReader = new();
    static readonly int[] producerNumbers = [1, 2, 3];
    static void Main(string[] args)
    {
        MainForDirectVersion1();
        //MainForFanOut();
    }

    private static void MainForDirectVersion1()
    {
        var producerSetting = new ProducerSettings(0, 0, "Direct", ProducerType.SimpleProducer, "", "Read console direct");
        Console.WriteLine("Starting creating producers");
        var producers = factory.Create(producerSetting)
            ?? throw new ApplicationException("Unable to create producer");
        foreach(var producer in producers)
        {
            Console.WriteLine($"Created producer '{producer.Name}' sending messages to exchange '{producerSetting.ExchangeName}'.");
        }
        
        while (_consoleReader.Read(out var message))
        {
            var actions = producers
                .Select<IProducer, Action>(p => () => p.Send(message))
                .ToArray();

            Parallel.Invoke(actions);
        }

        producers.First().Send("q");
        Console.WriteLine("Shutting down");
        Console.ReadLine();
    }
}
