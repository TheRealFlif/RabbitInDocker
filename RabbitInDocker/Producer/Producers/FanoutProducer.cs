using Producer.Entities;
using RabbitMQ.Client;

namespace Producer.Producers
{
    public class FanoutProducer : ProducerBase<string>
    {
        //private readonly IModel _channel;
        //private readonly ProducerSettings _producerSettings;

        public FanoutProducer(
            IModel channel, 
            ProducerSettings producerSettings) : base(channel, producerSettings) { }

        public override void Send(string? message)
        {
            Channel.ExchangeDeclare(
                Settings.ExchangeName, 
                ExchangeType.Fanout, 
                durable:true);
            Console.WriteLine($"{Settings.Name} sending '{message}'");
            base.Send(message);
            Console.WriteLine($"{Settings.Name} send '{message}'");
        }
    }
}
