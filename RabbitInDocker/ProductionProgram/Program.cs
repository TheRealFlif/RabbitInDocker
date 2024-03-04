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
            .Select(i => new ProducerSettings(1, 1, "Pubsub", "letterbox", $"#{i}"))
            .Select(factory.Create)
            .Select(p => p as FanoutProducer);

        var tasks = producers
            .Select(ap => Task.Run(() => {
                for (var i = 0; i < 10; i++)
                {
                    ap?.Send(Guid.NewGuid().ToString("N"));
                }
            }))
            .ToArray();

        Task.WaitAll(tasks);

        Console.WriteLine("All messages sent");
        
        Console.WriteLine("Write next messages to send (q to exit)");
        var readValue = Console.ReadLine();
        while (readValue != "q")
        {
            tasks = producers
            .Select(ap => Task.Run(() =>
            {
                for (var i = 0; i < (readValue?.Length ?? 1); i++)
                {
                    ap?.Send(Guid.NewGuid().ToString("N"));
                }
            }))
            .ToArray();
            Task.WaitAll(tasks);
            
            Console.WriteLine("All messages sent");
            
            Console.WriteLine("Write next messages to send (q to exit)");
            readValue = Console.ReadLine();
        }

        Task.WaitAny(producers.Select(ap => Task.Run(() => { ap.Send("q"); })).ToArray());

        Console.WriteLine("Shutting down");
        producers?.First()?.ShutDown();
        Console.ReadLine();
    }
}
