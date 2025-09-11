using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

class ProjUtils
    // for general utils
{
    public static bool loggingEnabled = true;

    // to bypass the limit
    private static string ReadLine()
    {
        StringBuilder read = new();
        StreamWriter stdout = new(Console.OpenStandardOutput());
        stdout.AutoFlush = true;

        ConsoleKeyInfo key;

        while (true)
        {
            key = Console.ReadKey(true);

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

    [Obsolete("not needed anymore")]
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

    private static ConcurrentQueue<string> askQueue = new();
    private static int asking = 0;
    public static bool AskOutput(string str)
    {
        askQueue.Enqueue(str);

        // only one thread at a time is allowed to drain the queue
        if (Interlocked.CompareExchange(ref asking, 1, 0) != 0)
            return false;


        while (askQueue.TryDequeue(out string? output))
        {
            Console.WriteLine($"Possible plaintext: {output} (Y/N)?");
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Y)
            {
                Interlocked.Exchange(ref asking, 0);
                return true;
            }
        }

        Interlocked.Exchange(ref asking, 0);
        return false;
    }

    [Obsolete("only for debugging")]
    public static Dictionary<string, Stopwatch> timers = new();
    [Obsolete("only for debugging")]
    public static void StartTimer(string name)
    {
        if (!loggingEnabled) { return; }
        if (timers.ContainsKey(name)) { return; }

        timers.Add(name, Stopwatch.StartNew());
    }
    [Obsolete("only for debugging")]

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

    public static void HandleSearchResult(bool success)
    {
        if (success)
        {
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.Gray;
            return;
        }

        // :(
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("sorry, i wasn't able to find a meaningful decryption =(");

        Console.ReadKey();
        Console.ForegroundColor = ConsoleColor.Gray;
    }
}
