using Cryptographer;
using Cryptographer.Utils;

class Program
{
    static void Main(string[] args)
    {
        // CONFIG
        Config.SetFromFile("config.ini");
        Config.SetFromCLI(args);

        Ngrams.Wake();

        // SEARCHER
        string input = PrintUtils.GetInput(Config.CLIargs);

        Searcher searcher = new();
        PrintUtils.DisplayWorkText(searcher);

        searcher.Search(input ?? "");

        PrintUtils.HandleSearchResult(searcher.status);
    }
}