namespace Producer.Entities;

public readonly record struct ProducerSettings (
    int MinWait, 
    int MaxWait, 
    string ExchangeName, 
    ProducerType TypeOfExchange,
    string RoutingKey, 
    string Name) 
{
    public ProducerSettings ChangeRoutingKey(string routingKey)
    {
        return new ProducerSettings(MinWait, MaxWait, ExchangeName, TypeOfExchange, routingKey, Name);
    }
}
