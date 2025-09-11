using Cryptographer;
using Cryptographer.Utils;

class Program
{
    static void Main(string[] args)
    {
        // CONFIG
        Dictionary<string, string> argDict = new();
        foreach (string s in args)
        {
            string[] split = s.Split("=");
            argDict.TryAdd(split[0], split[1]);
        }

        Config.SetFromFile(argDict.TryGetValue("cfg", out string? path) ? path : "config.ini");
        Config.Set(argDict); // override config with cli arguments

        // SEARCHER
        string input = ProjUtils.GetInput(argDict);

        Console.WriteLine("Working...");

        Searcher searcher = new();
        searcher.Search(input ?? "");

        ProjUtils.HandleSearchResult(searcher.success);
    }
}