using System;
using System.IO;

namespace Lira;

public class Lira
{

    public static void Main(string[] args)
    {
        Console.WriteLine("Hello World");
        if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            Console.WriteLine("Input a file");
        }
    }

    // Just prints the file for now
    public static void RunFile(string path)
    {
        string source = File.ReadAllText(path);
        Scanner scanner = new(source);
        List<Token> tokens = scanner.ScanTokens();
        foreach (Token token in tokens) Console.WriteLine(token);
    }

}
