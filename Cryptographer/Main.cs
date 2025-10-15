using Cryptographer;
using Cryptographer.Utils;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        // CONFIG
        Console.OutputEncoding = Encoding.UTF8;

        Config.SetFromFile("config.ini");
        Config.SetFromCLI(args);

        Ngrams.Wake();

        // SEARCHER
        string input = PrintUtils.GetInput(Config.CLIargs);
        string key = PrintUtils.GetKey(Config.CLIargs);

        Searcher searcher = new(input, key);
        PrintUtils.DisplayWorkText(searcher);

        searcher.Search();
        PrintUtils.HandleSearchResult(searcher.status);
    }
}