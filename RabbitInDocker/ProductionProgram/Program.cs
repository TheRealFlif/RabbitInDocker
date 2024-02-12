namespace ProductionProgram;

using Producer;
using Producer.Entities;
using Producer.Producers;

internal class Program
{
    static void Main(string[] args)
    {
        var factory = new ProducerFactory();

        var producers = new[] { 1, 2, 3 }
            .Select(i => new ProducerSettings(100, 1000, "letterbox", $"#{i}"))
            .Select(factory.Create)
            .Select(p => p as AutomaticProducer);

        var tasks = producers
            .Select(ap => Task.Run(() => ap?.SendMessages(10)))
            .ToArray();
        
        Task.WaitAll(tasks);

        Console.WriteLine("All messages sent");
        Console.WriteLine("Write next messages to send (q to exit)");
        var readValue = Console.ReadLine();
        while(readValue != "q")
        {
            tasks = producers
            .Select(ap => Task.Run(() => ap?.SendMessages(readValue?.Length ?? 0)))
            .ToArray();

            readValue = Console.ReadLine();
        }

        Console.WriteLine("Shutting down");
        producers?.First()?.ShutDown();
        Console.ReadLine();
    }
}
