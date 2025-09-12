using Cryptographer.Utils;
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

            char keyChar = key.KeyChar;
            if (keyChar == 0) continue; // dont stdout stuff like the windows key

            if (key.Key == ConsoleKey.Enter)
                break;

            read.Append(keyChar);
            stdout.Write(keyChar);
        }

        return read.ToString();
    }

    public static string GetInput(Dictionary<string, string> args)
    {
        if (args.TryGetValue("input", out string? input))
        {
            Console.WriteLine();
            if (!input.EndsWith(".txt"))
                return input;

            return File.ReadAllText($"{AppContext.BaseDirectory}/{input}");
        }

        Console.WriteLine("Input the text you want to decrypt:");
        input = ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Input can't be empty. Please try again.");
            return GetInput(args);
        }

        Console.WriteLine();
        return input;
    }

    private static ConcurrentQueue<string> askQueue = new();
    private static int asking = 0;
    public static bool AskOutput(string str, DecryptionBranch branch)
    {
        askQueue.Enqueue(str);

        // only one thread at a time is allowed to drain the queue
        if (Interlocked.CompareExchange(ref asking, 1, 0) != 0)
            return false;


        while (askQueue.TryDequeue(out string? output))
        {
            Console.WriteLine($"Possible plaintext: {output} (\x1b[92my\x1b[39m/\x1b[91mn\x1b[39m)?"); // ugly way to print green y and red n
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Y) continue;

            // then its plaintext
            if (Config.showStackTrace)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Methods used:");
                Console.WriteLine($" -{branch.Method.Name}");

                DecryptionNode node = branch.Parent;
                while (node.Parent != null && node.Parent.Method != "")
                {
                    Console.WriteLine($" -{node.Parent.Method}");
                    node = node.Parent;
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Interlocked.Exchange(ref asking, 0);
            return true;
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
