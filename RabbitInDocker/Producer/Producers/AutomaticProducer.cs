#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Producer.Entities;
using RabbitMQ.Client;

namespace Producer.Producers;

/// <summary>
/// Creates messages automtically with a randomized interval
/// </summary>
public class AutomaticProducer : ProducerBase<string>
{
    public AutomaticProducer(IModel channel, ProducerSettings producerSettings) : base(channel, producerSettings) { }

    public override void ShutDown()
    {
        Send("q");
    }
}
