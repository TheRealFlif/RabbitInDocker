using Consumer.Consumers;
using Producer.Entities;

namespace Consumer;

public interface IConsumerFactory
{
    public IConsumer Create(ConsumerSettings? consumerSettings);
}
