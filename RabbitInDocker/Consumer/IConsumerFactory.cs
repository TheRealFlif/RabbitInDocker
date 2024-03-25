using Consumer.Consumers;
using Producer.Entities;

namespace Consumer;

public interface IConsumerFactory
{
    public IEnumerable<IConsumer> Create(ConsumerSettings consumerSettings);
}
