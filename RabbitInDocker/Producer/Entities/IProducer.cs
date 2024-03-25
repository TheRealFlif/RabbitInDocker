namespace Producer.Entities;

public interface IProducer
{
    public void Send(string data);
}
