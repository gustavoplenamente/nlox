namespace nlox;

public class Lox
{
    private static bool HadError { get; set; }
    private IScanner Scanner { get; }
    private IWriter Writer { get; }

    public Lox(IScanner scanner, IWriter writer)
    {
        Scanner = scanner;
        Writer = writer;
    }

    public void Run()
    {
        var tokens = Scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Writer.Write(token);
        }
    }

    public static void Error(int line, string message) {
        Report(line, "", message);
    }

    public static void Report(int line, string where, string message)
    {
        Console.WriteLine($"[line {line}] Error{where}: {message}");
        HadError = true;
    }
}