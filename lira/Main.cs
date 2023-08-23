namespace Lira;

public class Lira
{

    public static void Main(string[] args)
    {
        if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            Console.WriteLine("""
            Welcome to Lira habibi!
            We charge 10% interest in performance for each line of text.
            ------------------------------------------------------------
            Usage: lira <file_name>
            """);
        }
    }

    public static void RunFile(string path)
    {
        string source = File.ReadAllText(path);
        Scanner scanner = new(source);
        List<Token> tokens = scanner.ScanTokens();
        
        Console.WriteLine("Scanner returned following tokens:");
        foreach(Token token in tokens) Console.Write($"{token} ");

        Console.WriteLine("\n");

        Parser parser = new(tokens);
        IExpr? expr = parser.Parse();

        if (expr is null)
        {
            Console.Write("Parser returned null expression.");
            return;
        }
        Console.WriteLine(new ASTPrinter().Print(expr));
        
        Console.WriteLine("\n");

        Interpreter interpreter = new();
        interpreter.Interpret(expr);
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Report(int line, string context, string message)
    {
        Console.WriteLine($"[line {line}] Error {context}: {message}");
    }

}
