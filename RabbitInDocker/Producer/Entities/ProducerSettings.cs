namespace Producer.Entities;

public readonly record struct ProducerSettings (
    int MinWait, 
    int MaxWait, 
    string ExchangeName, 
    TypeOfExchange TypeOfExchange,
    string RoutingKey, 
    string Name) { }
