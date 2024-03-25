namespace ProductionProgram;

internal class ConsoleReader
{
    internal bool Read(out string message)
    {
        Console.Write("Write next messages to send (q to exit): ");
        message = Console.ReadLine() ?? "q";
        return string.Compare(message, "q", StringComparison.InvariantCultureIgnoreCase) != 0;
    }
}
