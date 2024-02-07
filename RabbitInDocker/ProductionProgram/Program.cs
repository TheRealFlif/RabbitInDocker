namespace ProductionProgram;

internal class Program
{
    static void Main(string[] args)
    {
        var producer = new Producer.AutomaticProducer(100, 1000, "letterbox", "#1");
        var producer1 = new Producer.AutomaticProducer(100, 1000, "letterbox", "#2");
        var producer2 = new Producer.AutomaticProducer(100, 1000, "letterbox", "#3");
        
        Console.WriteLine("Sending ten messages");
        var t = Task.Run(() => { producer.SendMessages(10); });
        Console.WriteLine("Ten more");
        var t1 = Task.Run(() => { producer1.SendMessages(10); });
        Console.WriteLine("Another ten more ");
        var t2 = Task.Run(() => { producer2.SendMessages(10); });
        Task.WaitAll([t, t1, t2]);
        Console.WriteLine("Shutting down");
        producer.ShutDown();
        Console.ReadLine();
    }
}
