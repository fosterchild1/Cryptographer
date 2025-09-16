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

        List<char> things = new() { '|', '/', '-', '\\', '|', '/', '-', '\\' };
        Thread thr = new(() =>
        {
            int thing = 1;
            while (searcher.status == searchStatus.SEARCHING)
            {
                Thread.Sleep(250);
                if (ProjUtils.asking > 0) continue;

                CLIUtils.ClearLine();
                Console.Write($"{things[thing]} Working");
                thing = (thing + 1) % (things.Count);
            }
        });
        thr.Start();

        searcher.Search(input ?? "");

        ProjUtils.HandleSearchResult(searcher.status);
    }
}