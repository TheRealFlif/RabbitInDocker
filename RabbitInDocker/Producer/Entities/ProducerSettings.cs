namespace Producer.Entities;

public readonly record struct ProducerSettings (
    int MinWait, 
    int MaxWait, 
    string ExchangeName, 
    string RoutingKey, 
    string Name) { }
