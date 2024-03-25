namespace ProductionProgram;

internal class ConsoleReader
{
    internal bool Read(out string message)
    {
        message = Console.ReadLine() ?? "q";
        return string.Compare(message, "q", StringComparison.InvariantCultureIgnoreCase) != 0;
    }
}
