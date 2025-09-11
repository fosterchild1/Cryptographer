using Cryptographer;
using Cryptographer.Utils;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // CONFIG
        Dictionary<string, string> argDict = new();
        foreach (string s in args)
        {
            string[] split = s.Split("=");
            argDict.TryAdd(split[0], split[1]);
        }

        if (args.Length > 0)
            Config.Set(argDict);
        else
            Config.SetFromFile("config.ini");

        // SEARCHER
        string? input = "";
        if (!argDict.TryGetValue("input", out input))
            input = ProjUtils.GetInput();

        Console.Clear();
        Console.WriteLine("Working...");

        Searcher searcher = new();
        searcher.Search(input ?? "");

        if (Searcher.success) {
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