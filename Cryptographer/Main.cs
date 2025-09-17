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

        Searcher searcher = new();
        ProjUtils.DisplayWorkText(searcher);

        searcher.Search(input ?? "");

        ProjUtils.HandleSearchResult(searcher.status);
    }
}