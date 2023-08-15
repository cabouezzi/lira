using System;
using System.IO;

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
        foreach (Token token in tokens) Console.WriteLine(token);
    }

}
