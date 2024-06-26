﻿using Consumer;
using Consumer.Consumers;
using Producer.Entities;

namespace ConsumerProgram;

internal class Program
{
    readonly static System.Collections.Concurrent.BlockingCollection<IConsumer> _consumers = [];

    static void Main(string[] args)
    {
        Thread.Sleep(4000);
        //MainForSimpleConsumer();
        //MainForSleepingConsumer();
        //MainForSubscriberConsumers();
        //MainForRoutingConsumer();
        MainForTopicConsumer();
    }

    private static void MainForSimpleConsumer()
    {
        var factory = new ConsumerFactory(ExitMessageReceived);
        var settings = new ConsumerSettings(1, 1000, "directQueue", "SimpleConsumer", 1) { 
            ExchangeName = "Direct",
            ConsumerType = ConsumerType.SimpleConsumer
        };
        Console.WriteLine("Starting consumers");
        var consumers = factory.Create(settings);
        foreach (var consumer in consumers)
        {
            Console.WriteLine($"Consumer '{consumer.Name}' created and is listing to queue '{consumer.QueueName}'.");
        }
         while (true) ;
    }

    private static void MainForSleepingConsumer()
    {
        var factory = new ConsumerFactory(ExitMessageReceived);
        var settings = new ConsumerSettings(1, 1000, "directQueue", string.Empty, 1)
        {
            ExchangeName = "Direct",
            ConsumerType = ConsumerType.SleepingConsumer,
        };
        Console.WriteLine("Starting consumers");
        var consumers = factory.Create(settings);
        foreach (var consumer in consumers)
        {
            Console.WriteLine($"Consumer '{consumer.Name}' created and is listing to queue '{consumer.QueueName}'.");
        }
        while (true) ;
    }

    private static void MainForSubscriberConsumers()
    {
        var factory = new ConsumerFactory(ExitMessageReceived);
        var settings = new ConsumerSettings(1, 1000, string.Empty, string.Empty, 1)
        {
            ExchangeName = "PubSub",
            ConsumerType = ConsumerType.Subscriber,
        };
        Console.WriteLine("Starting consumers");
        var consumers = factory.Create(settings);
        foreach (var consumer in consumers)
        {
            Console.WriteLine($"Consumer '{consumer.Name}' created and is listing to queue '{consumer.QueueName}'.");
        }
        while (true) ;
    }

    private static void MainForRoutingConsumer()
    {
        var factory = new ConsumerFactory(ExitMessageReceived);
        var settings = new ConsumerSettings(1, 1000, "", "SimpleConsumer", 1)
        {
            ExchangeName = "Routing",
            ConsumerType = ConsumerType.Routing
        };
        Console.WriteLine("Starting consumers");
        var consumers = factory.Create(settings);
        foreach (var consumer in consumers)
        {
            Console.WriteLine($"Consumer '{consumer.Name}' created and is listing to queue '{consumer.QueueName}'.");
        }
        while (true) ;
    }

    private static void MainForTopicConsumer()
    {
        var factory = new ConsumerFactory(ExitMessageReceived);
        var settings = new ConsumerSettings(1, 1000, "", "SimpleConsumer", 1)
        {
            ExchangeName = "Topic",
            ConsumerType = ConsumerType.Topic
        };
        Console.WriteLine("Starting consumers");
        var consumers = factory.Create(settings);
        foreach (var consumer in consumers)
        {
            Console.WriteLine($"Consumer '{consumer.Name}' created and is listing to queue '{consumer.QueueName}'.");
        }
        while (true) ;
    }

    private static void ExitMessageReceived(object? sender, EventArgs e)
    {
        Environment.Exit(0);
    }
}

