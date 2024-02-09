namespace Consumer.Consumers
{
    public interface IConsumer : IDisposable
    {
        public event EventHandler ExitMessageReceived;
    }
}
