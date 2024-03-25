#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Producer.Entities;
using RabbitMQ.Client;

namespace Producer.Producers;

public abstract class ProducerBase<T> : IProducer, IDisposable
{
    private int _messageNumber = 0;
    internal string Name { get; }
    protected WaitTimeCreator WaitTimeCreator { get; set; }
    private LatinWordCreator _latinWordCreator = new LatinWordCreator();
    protected IModel Channel { get; init; }
    protected ProducerSettings Settings { get; init; }

    public ProducerBase(
        IModel channel,
        ProducerSettings settings)
    {
        Channel = channel;
        Settings = settings;
        Name = _latinWordCreator.CreateCombination(2, ' ');
    }

    public virtual void Send(string data)
    {
        var envelope = Create(data);
        var body = System.Text.Encoding.UTF8.GetBytes(envelope.To());
        if (envelope != null)
        {
            Channel.BasicPublish(
                Settings.ExchangeName,
                Settings.RoutingKey,
                null,
                body);
        }
        Sleep();
    }

    protected virtual Envelope<string>? Create(string data)
    {
        var newData = data ?? string.Empty;

        var returnValue = new Envelope<string>(newData);
        returnValue["sender"] = Name;
        returnValue["messageNumber"] = $"{++_messageNumber:00}";

        return returnValue;
    }

    public virtual void ShutDown() { }

    protected void Sleep()
    {
        if (WaitTimeCreator != null)
        {
            Thread.Sleep(WaitTimeCreator.GetMilliseconds());
        }
    }

    private bool _disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        //if (!_disposedValue)
        //{
        //    if (disposing)
        //    {
        //        Channel.Dispose();
        //    }

        //    _disposedValue = true;
        //}
    }
    public void Dispose()
    {
        //Dispose(disposing: true);
        //GC.SuppressFinalize(this);
    }
}
