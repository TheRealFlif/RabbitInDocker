namespace Producer.Entities;

public readonly struct ConsumerSettings { 

    public int MinWait { get; }
    
    public int MaxWait { get; }
    
    public string QueueName { get; }

    public string Name { get; }

    public ushort PrefetchCount { get; }

    public ConsumerSettings(
        int minWait, 
        int maxWait, 
        string queueName, 
        string name, 
        ushort prefetchCount) {

        MinWait = minWait < maxWait ? minWait : maxWait;
        MaxWait = maxWait > minWait ? maxWait : minWait;
        QueueName = queueName;
        Name = name;
        PrefetchCount = prefetchCount;
    }
    
}
