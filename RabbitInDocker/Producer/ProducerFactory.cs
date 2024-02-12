namespace Producer;

using Producer.Entities;
using Producer.Producers;

public class ProducerFactory
{
    public IProducer Create(ProducerSettings settings)
    {
        return new AutomaticProducer(settings.MinWait, settings.MaxWait, settings.RoutingKey, settings.Name);
    }
}
