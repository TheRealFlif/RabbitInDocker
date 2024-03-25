namespace Producer.Entities;

public struct ConsumerSettings
{
    private ConsumerType _consumerType;

    public int MinWait { get; }

    public int MaxWait { get; }

    public string Name { get; set; }

    public string ExchangeName { get; set; }

    public string QueueName { get; }

    public string RoutingKey { get; set; }

    public ushort PrefetchCount { get; }

    public ConsumerType ConsumerType
    { readonly get => _consumerType;
        set => _consumerType = Enum.IsDefined(typeof(ConsumerType), value)
            ? value
            : ConsumerType.Unknown;
    }

    public ConsumerSettings(
        int minWait,
        int maxWait,
        string queueName,
        string name,
        ushort prefetchCount)
    {

        MinWait = minWait < maxWait ? minWait : maxWait;
        MaxWait = maxWait > minWait ? maxWait : minWait;
        QueueName = queueName;
        ExchangeName = string.Empty;
        RoutingKey = string.Empty;
        Name = name;
        PrefetchCount = prefetchCount;
    }

    public static ConsumerSettings SubscriberSettings(string name, string exchangeName, string routingKey)
    {
        var returnValue = new ConsumerSettings(0, 0, string.Empty, name, 1)
        {
            ConsumerType = ConsumerType.Subscriber,
            ExchangeName = exchangeName ?? string.Empty,
            RoutingKey = routingKey ?? string.Empty
        };

        return returnValue;
    }
}
