using nlox;

var hadError = false;

if (args.Length > 1)
{
    Console.WriteLine("Usage: nlox [script path]");
    Environment.Exit(64);
} 

if (args.Length == 1)
{
    RunFile(args[0]);
    Environment.Exit(0);
}

RunPrompt();
Environment.Exit(0);
return;

void RunFile(string scriptPath)
{
    var text = File.ReadAllText(scriptPath);
    var lox = new Lox(new Scanner(text));
    lox.Run();
    if (hadError) Environment.Exit(65);
}

void RunPrompt()
{
    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        if (input is null) break;

        var lox = new Lox(new Scanner(input));
        lox.Run();
        hadError = false;
    }
}
