using Consumer.Consumers;

namespace Consumer
{
    public interface IConsumerFactory
    {
        public IConsumer Create(string queueName);
    }
}
