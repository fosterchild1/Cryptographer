using Cryptographer;
using Cryptographer.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

class PrintUtils
{
    // CLI UTILS
    private static string C_RED = "\x1b[91m";
    private static string C_GREEN = "\x1b[92m";
    private static string C_DARKYELLOW = "\x1b[33m";
    private static string C_YELLOW = "\x1b[93m";
    private static string C_GRAY = "\x1b[37m";
    private static void ClearLine()
    {
        int top = Console.CursorTop;

        Console.SetCursorPosition(0, top);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, top);
    }

    /// <summary>Console.ReadKey() except it doesn't detect stuff like the Win key</summary>
    private static void ReadUntilValidKey()
    {
        while (true)
        {
            ConsoleKeyInfo k = Console.ReadKey();
            if (k.KeyChar != 0)
                break;
        }
    }

    // PRINTUTILS
    public static string GetInput(Dictionary<string, string> args)
    {
        EnableColors();

        if (args.TryGetValue("in", out string? input))
        {
            Console.WriteLine();
            if (!input.EndsWith(".txt"))
                return input;

            return File.ReadAllText($"{AppContext.BaseDirectory}/{input}");
        }

        Console.WriteLine("Input the text you want to decrypt:");
        input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Input can't be empty. Please try again.");
            return GetInput(args);
        }

        Console.WriteLine();
        return input;
    }

    public static string GetKey(Dictionary<string, string> args)
    {
        if (args.TryGetValue("key", out string? key))
        {
            Config.UseKey = true;
            return key;
        }

        if (!Config.UseKey)
            return "";

        Console.WriteLine("Input the suspected key (if you don't have any, leave blank):");
        key = Console.ReadLine();

        Console.WriteLine();
        return key ?? "";
    }

    private static ConcurrentQueue<string> askQueue = new();
    public static int asking = 0;
    public static bool AskOutput(string str, DecryptionBranch branch)
    {
        askQueue.Enqueue(str);

        // only one thread at a time is allowed to drain the queue
        if (Interlocked.CompareExchange(ref asking, 1, 0) != 0)
            return false;

        ClearLine();
        while (askQueue.TryDequeue(out string? output))
        {
            Console.Write($"Possible plaintext: {output} ({C_GREEN}y{C_GRAY}/{C_RED}n{C_GRAY})?");

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Y)
            {
                ClearLine();
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
                    node = node.Parent!;
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
        if (timers.ContainsKey(name)) { return; }

        timers.Add(name, Stopwatch.StartNew());
    }
    [Obsolete("only for debugging")]

    public static void StopTimer(string name)
    {
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

                // change console name
                Console.Title = $"Cryptographer {Config.version} | {searcher.totalDecryptions} Decryptions";

                if ((int)searcher.status > 2) break;  // < 2 means not started or searching
                if (asking > 0) continue;

                // write Working...
                ClearLine();
                Console.Write($"{things[thing]} Working");
                thing = (thing + 1) % (things.Count);
            }

        }).Start();
    }

    public static void HandleSearchResult(searchStatus status)
    {
        ClearLine();

        if (status == searchStatus.SUCCESS)
        {
            ReadUntilValidKey();
            Console.ForegroundColor = ConsoleColor.Gray;
            return;
        }

        // :(
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("sorry, i wasn't able to find a meaningful decryption =(");

        ReadUntilValidKey();
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void DisplayHelpText()
    {
        Console.WriteLine("Cryptographer\n" +
            "Source: https://github.com/fosterchild1/Cryptographer\n\n" +
            "Example usage: cryptographer in=aHR0cHM6Ly9naXRodWIuY29tL2Zvc3RlcmNoaWxkMS8 maxdepth=2\n\n" +
            "Configuration:\n" +
            "Config Name | Default | What it does\n" +
            "h / help                Displays the help text.\n" +
            "plaintext=    float     Minimum score needed for an output to be considered plaintext, 0 = Default.\n" +
            "maxdepth=     byte      The max depth of the search, 0 = Default.\n" +
            "threads=      byte      Amount of cpu cores to be used by the program, 0 = all of them. 1 thread is almost always enough.\n" +
            "trigrams=     boolean   Use trigrams instead of quadgrams when determining plaintext.\n" +
            "stacktrace=   boolean   Shows the ciphers used to get to the plaintext.\n" +
            "timeout=      int       Max time the search can go on for (in seconds), 0 = Default.\n" +
            "usekey=       boolean   Enables keyed ciphers such as Vigenère.\n"
        );

        Environment.Exit(0);
    }

    public static void PrintDbgDecryption(DecryptionBranch branch, int workerIndex)
    {
        string text = branch.Parent.Text;
        string truncated = text.Length >= 60 ? $"{text.AsSpan(0, 60).ToString()}.. [truncated]" : text;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"APPLIED: {C_DARKYELLOW}{branch.Method.Name} {C_YELLOW}| ON: {C_DARKYELLOW}{'"'}{truncated}{'"'} {C_YELLOW}| AT DEPTH: {C_DARKYELLOW}" +
            $"{branch.Parent.Depth}");
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    // ENABLE COLORS
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

    public static void EnableColors()
    {
        nint handle = GetStdHandle(-11);
        if (!GetConsoleMode(handle, out int mode)) return;

        SetConsoleMode(handle, mode | 0x0004);
    }
}
