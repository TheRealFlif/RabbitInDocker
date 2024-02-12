namespace Producer.Entities;

public readonly record struct ProducerSettings (int MinWait, int MaxWait, string RoutingKey, string Name) { }
