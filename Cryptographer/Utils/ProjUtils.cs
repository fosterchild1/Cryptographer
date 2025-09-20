using Cryptographer;
using Cryptographer.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

class ProjUtils
    // for general utils
{
    public static bool loggingEnabled = true;

    public static string GetInput(Dictionary<string, string> args)
    {
        if (args.TryGetValue("in", out string? input))
        {
            Console.WriteLine();
            if (!input.EndsWith(".txt"))
                return input;

            return File.ReadAllText($"{AppContext.BaseDirectory}/{input}");
        }

        Console.WriteLine("Input the text you want to decrypt:");
        input = CLIUtils.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Input can't be empty. Please try again.");
            return GetInput(args);
        }

        Console.WriteLine();
        return input;
    }

    private static ConcurrentQueue<string> askQueue = new();
    public static int asking = 0;
    public static bool AskOutput(string str, DecryptionBranch branch)
    {
        askQueue.Enqueue(str);

        // only one thread at a time is allowed to drain the queue
        if (Interlocked.CompareExchange(ref asking, 1, 0) != 0)
            return false;

        CLIUtils.ClearLine();
        while (askQueue.TryDequeue(out string? output))
        {
            Console.Write($"Possible plaintext: {output} (\x1b[92my\x1b[39m/\x1b[91mn\x1b[39m)?"); // ugly way to print green y and red n
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Y)
            {
                CLIUtils.ClearLine();
                continue;
            };

            // then its plaintext
            if (Config.showStackTrace)
            {
                // show cipher used to get to the plaintext by backtracking up the tree
                Console.ForegroundColor = ConsoleColor.Blue;

                List<string> methods = new() { branch.Method.Name };

                DecryptionNode node = branch.Parent;
                while (node.Parent != null && node.Parent.Method != "")
                {
                    methods.Add(node.Parent.Method);
                    node = node.Parent;
                }

                Console.WriteLine("\nMethods Used:");
                for (int i=methods.Count - 1; i>=0; i--)
                    Console.WriteLine($" -{methods[i]}");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
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

        Console.WriteLine($"{name}: {timer.ElapsedMilliseconds}ms");
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

    public static void DisplayWorkText(Searcher searcher)
    {
        List<char> things = new() { '|', '/', '-', '\\', '|', '/', '-', '\\' };
        new Thread(() =>
        {
            int thing = 1;
            while ((int)searcher.status < 2)
            {
                Thread.Sleep(250);
                if (ProjUtils.asking > 0 || (int)searcher.status > 2) continue;  // < 2 means not started or searching

                CLIUtils.ClearLine();
                Console.Write($"{things[thing]} Working");
                thing = (thing + 1) % (things.Count);
            }

            Console.Write('\n');
            CLIUtils.ClearLine();
        }).Start();
    }

    public static void HandleSearchResult(searchStatus status)
    {
        if (status == searchStatus.SUCCESS)
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
