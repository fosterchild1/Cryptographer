using Cryptographer;
using Cryptographer.DecryptionMethods;
using Cryptographer.Utils;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;

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

        List<string> outputs = Searcher.Search(input);

        Dictionary<string, float> scoreDict = new();

        foreach (string output in outputs)
        {
            float score = StringScorer.Score(output, FrequencyAnalysis.AnalyzeFrequency(output));
            if (score < Constants.scorePrintThreshold) continue;
            scoreDict[output] = score;
        }

        // :(
        if (scoreDict.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("sorry, i wasn't able to find a meaningful decryption =(");
            return;
        }

        Console.WriteLine("Possible outputs:");
        foreach (KeyValuePair<string, float> output in scoreDict.OrderBy(kv => kv.Value))
        {
            Console.WriteLine($"{output.Key} ({output.Value})");
        }

        Console.Read();
    }
}