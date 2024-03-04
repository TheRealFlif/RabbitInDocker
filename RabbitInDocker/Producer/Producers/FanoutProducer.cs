using Producer.Entities;
using RabbitMQ.Client;

namespace Producer.Producers
{
    public class FanoutProducer : ProducerBase<string>
    {
        public FanoutProducer(
            IModel channel, 
            ProducerSettings producerSettings) : base(channel, producerSettings) { }

        public override void Send(string? message)
        {
            Console.WriteLine($"{Settings.Name} sending '{message}'");
            base.Send(message);
            Console.WriteLine($"{Settings.Name} send '{message}'");
        }
    }
}
