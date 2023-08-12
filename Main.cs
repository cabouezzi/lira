using System;
using System.IO;

public class Lira
{

    public static void Main (String[] args)
    {
        Console.WriteLine("Hello World");
        if (args.Length == 1)
        {
            runFile(args[0]);
        }
        else {
            Console.WriteLine("Input a file");
        }
    }

    // Just prints the file for now
    public static void runFile (String path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        foreach (byte symbol in bytes) 
        {
            Console.Write("{0}", (char)symbol);
        }
    }

}