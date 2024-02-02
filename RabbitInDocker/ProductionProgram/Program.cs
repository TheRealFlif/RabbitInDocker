namespace ProductionProgram;

internal class Program
{
    static void Main(string[] args)
    {
        using var producer = new Producer.Producer();
        var running = true;
        while(running)
        {
            Console.WriteLine("Enter message (Q = quit):");
            var message = Console.ReadLine();
            producer.SendMessage(message);

            if (message == "q")
            {
                break;
            }
        }
    }
}