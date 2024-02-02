using Consumer;

namespace ConsumerProgram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConsumerFactory();
            using var consumer = factory.Create("letterbox");
            var running = true;
            
            while (running)
            {

            }
        }
    }
}
