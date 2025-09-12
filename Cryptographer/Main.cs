using Cryptographer;
using Cryptographer.Utils;

class Program
{
    static void Main(string[] args)
    {
        // CONFIG
        Config.SetFromFile("config.ini");
        Config.SetFromCLI(args);

        // SEARCHER
        string input = ProjUtils.GetInput(Config.CLIargs);

        Console.WriteLine("Working...");

        Searcher searcher = new();
        searcher.Search(input ?? "");

        ProjUtils.HandleSearchResult(searcher.success);
    }
}