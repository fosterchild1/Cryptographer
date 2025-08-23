using Cryptographer;
using Cryptographer.Utils;

class Program
{
    static void Main(string[] args)
    {

        Dictionary<string, string?> argDict = args.Select(a => a.Split('='))
                     .ToDictionary(a => a[0], a => a.Length == 2 ? a[1] : null);

        if (args.Length > 0)
            Constants.Set(argDict);

        Console.WriteLine("cryptographer is a tool for decrypting text");

        string? input = "";
        if (!argDict.TryGetValue("input", out input))
            input = ProjUtils.GetInput();
        Console.Clear();

        argDict.TryGetValue("maxdepth", out string? depth);
        Constants.maxDepth = byte.TryParse(depth, out byte byteDepth) ? byteDepth : ProjUtils.GetDepth();

        Console.Clear();


        Console.WriteLine("Working...");

        Searcher.Search(input ?? "");

        // :(
        if (Searcher.success) { Console.ReadKey(); return; }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("sorry, i wasn't able to find a meaningful decryption =(");
        Console.ReadKey();
    }
}