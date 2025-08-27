using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ProjUtils
    // for general utils
{
    public static bool loggingEnabled = true;

    // to bypass the limit
    private static string ReadLine()
    {
        StringBuilder read = new StringBuilder();
        StreamWriter stdout = new StreamWriter(Console.OpenStandardOutput());

        ConsoleKeyInfo key;

        while (true)
        {
            key = Console.ReadKey();

            if (key.Key == ConsoleKey.Enter)
                break;

            read.Append(key.KeyChar);
            stdout.Write(key.KeyChar);
        }
        

        return read.ToString();
    }

    public static string GetInput()
    {
        Console.WriteLine("Please input the text you want to decrypt:");
        string? input = ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Input can't be empty. Please try again.");
            return GetInput();
        }

        return input;
    }

    [Obsolete]
    public static byte GetDepth()
    {
        Console.WriteLine("Please input the maximum depth of the search:");

        //is something?
        string? inputLine = ReadLine();
        if (string.IsNullOrEmpty(inputLine))
        {
            Console.WriteLine("Depth can't be empty. Please try again.");
            return GetDepth();
        }

        // is integer?
        bool isInt = int.TryParse(inputLine, out int input);
        if (!isInt)
        {
            Console.WriteLine("Input isn't an integer. Please try again.");
            return GetDepth();
        }
        return (byte)Math.Max(input, 1);
    }

    public static Dictionary<string, Stopwatch> timers = new();
    public static void StartTimer(string name)
    {
        if (!loggingEnabled) { return; }
        if (timers.ContainsKey(name)) { return; }

        timers.Add(name, Stopwatch.StartNew());
    }

    public static void StopTimer(string name)
    {
        if (!loggingEnabled) return;

        Stopwatch? timer;
        if (!timers.TryGetValue(name, out timer)) return;

        timer.Stop();
        timers.Remove(name);

        Console.WriteLine($"{name}: {timer.Elapsed.Nanoseconds}");
    }
    public static string RemoveWhitespaces(string input)
    {
        StringBuilder withoutWhitespaces = new();

        foreach (char c in input)
        {
            if (char.IsWhiteSpace(c)) continue;

            withoutWhitespaces.Append(c);
        }

        return withoutWhitespaces.ToString();
    }
}
