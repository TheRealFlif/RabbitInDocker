namespace Consumer.Consumers
{
    public interface IConsumer : IDisposable
    {
        public event EventHandler ExitMessageReceived;

        public string Name { get; }
        public string QueueName { get; }
    }
}
