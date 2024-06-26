﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Producer.Entities;
using RabbitMQ.Client;

namespace Producer.Producers;

public class SimpleProducer : ProducerBase<string>
{
    /// <summary>
    /// 
    /// </summary>
    public SimpleProducer(IModel channel, ProducerSettings settings) : base(channel, settings) { }

    public override void Send(string data)
    {
        var routingKey = Settings.RoutingKey;
        var newData = string.IsNullOrEmpty(routingKey) || string.Compare(data, "q", StringComparison.InvariantCultureIgnoreCase) == 0
            ? data
            : $"{routingKey}: {data}";
        base.Send(newData);

    }
}
