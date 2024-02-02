using Consumer;

namespace ConsumerProgram
{
    internal class Program
    {        
        static void Main(string[] args)
        {
            var factory = new ConsumerFactory();
            using var consumer = factory.Create("letterbox");
            if(consumer is DefaultConsumer defaultConsumer)
            {
                defaultConsumer.ExitMessageReceived += DefaultConsumer_ExitMessageReceived;
            }
            var running = true;
            
            while (running)
            {

            }
        }

        private static void DefaultConsumer_ExitMessageReceived(object? sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
