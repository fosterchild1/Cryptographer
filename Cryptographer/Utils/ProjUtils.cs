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
            // THE ABSOLUTE MOST HORRENDOUS WAY TO PRINT A GREEN Y AND RED N. i know that we can do it in 1 .Write statement,
            // but not all consoles support that
            Console.Write($"Possible plaintext: {output} ("); Console.ForegroundColor = ConsoleColor.Green; Console.Write("y"); 
            Console.ForegroundColor = ConsoleColor.Green; Console.Write("/"); Console.ForegroundColor = ConsoleColor.Red; Console.Write("n");
            Console.ForegroundColor = ConsoleColor.Gray; Console.Write(")?");

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

                while (node != null && node.Method != "")
                {
                    methods.Add(node.Method);
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

    public static void DisplayHelpText()
    {
        Console.WriteLine("Cryptographer\n" +
            "Source: https://github.com/fosterchild1/Cryptographer\n\n" +
            "Example usage: cryptographer in=aHR0cHM6Ly9naXRodWIuY29tL2Zvc3RlcmNoaWxkMS8 maxdepth=2\n\n" +
            "Configuration:\n" +
            "Config Name | Default | What it does\n" +
            "h  help               Displays the help text.\n" +
            "plaintext=   float    Minimum score needed for an output to be considered plaintext, 0 = Default.\n" +
            "maxdepth=    byte     The max depth of the search, 0 = Default.\n" +
            "threads=     byte     Amount of cpu cores to be used by the program, 0 = all of them. 1 thread is almost always enough.\n" +
            "trigrams=    boolean  Use trigrams instead of quadgrams when determining plaintext.\n" +
            "stacktrace=  boolean  Shows the ciphers used to get to the plaintext.\n" +
            "timeout=     int      Max time the search can go on for (in seconds), 0 = Default\n"
        );

        Environment.Exit(0);
    }
}
