﻿namespace ProductionProgram;

using Producer;
using Producer.Entities;
using Producer.Producers;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        var producers = factory.Create(producerSetting)
            ?? throw new ApplicationException("Unable to create producer");
        
        Console.WriteLine("Write next messages to send (q to exit)");
        
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

    private static void MainForFanOut()
    {
        var factory = new ProducerFactory();

        var producers = producerNumbers
            .Select(i => new ProducerSettings(1, 1, "Pubsub", ProducerType.FanOut, "letterbox", $"#{i}"))
            .Select(factory.Create)
            .Select(p => p as FanoutProducer)
           .ToArray();

        var tasks = producers
            .Select(ap => Task.Run(() =>
            {
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

        Task.WaitAny(producers.Select(ap => Task.Run(() => { ap?.Send("q"); })).ToArray());

        Console.WriteLine("Shutting down");
        producers?.First()?.ShutDown();
        Console.ReadLine();
    }
}

internal class ConsoleReader
{
    internal bool Read(out string message)
    {
        message = Console.ReadLine() ?? "q";
        return string.Compare(message, "q", StringComparison.InvariantCultureIgnoreCase) != 0; 
    }

}
