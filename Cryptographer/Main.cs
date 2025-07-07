using Cryptographer.DecryptionMethods;
using Cryptographer.DecryptionUtils;
using Cryptographer.Utils;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

class Program
{

    private static List<IDecryptionMethod> methods = new()
    {
        new Reverse(),
        new Atbash(),
        new Base64(),
        new Morse(),
        new Baconian(),
        new KeyboardSubstitution(),
    };

    private static int maxDepth = 0;

    // cache inputs and their outputs
    private static Dictionary<string, List<string>> memoCache = new();
    private static Dictionary<string, int> seenInputs = new();

    public static bool CheckOutput(string output, int depth = 1)
    {
        // aka useless string
        if (output.Replace(" ", "").Length <= 2)
            return false;

        bool found = seenInputs.TryGetValue(output, out int seenDepth);
        if (found && seenDepth <= depth)
            return false;

        return true;
    }

    public static List<string> GetDecrypted(string input, int depth = 1, string lastMethod = "")
    {
        // see if we ve met this input before
        List<string>? values;
        if (memoCache.TryGetValue(input, out values))
        {
            return values;
        }

        // dont go past max depth
        if (depth > maxDepth)
            return new List<string> { input };

        List<KeyValuePair<char, int>> analysis = FrequencyAnalysis.AnalyzeFrequency(input);
        List<string> decrypted = new();

        // backtrack methods
        foreach (IDecryptionMethod method in methods)
        {
            if (lastMethod == method.Name && (method.Name == "Reverse" || method.Name == "Atbash"))
                continue;

            List<string> outputs = method.Decrypt(input, analysis);

            foreach (string output in outputs)
            {
                if (!CheckOutput(output, depth)) continue;
                decrypted.AddRange(GetDecrypted(output, depth + 1, method.Name));
            }
        }

        // add to seen
        memoCache.TryAdd(input, decrypted);
        foreach (string output in decrypted)
        {
            seenInputs.TryAdd(output, depth);
        }

        return decrypted;
    }

    static void Main(string[] args)
    {
        Console.WriteLine("cryptographer is a tool for decrypting text");

        string input = Utils.GetInput();
        Console.Clear();
        maxDepth = Utils.GetDepth();
        Console.Clear();
        Console.WriteLine("Working...");

        List<string> outputs = GetDecrypted(input, 1);
        HashSet<string> noDupes = outputs.ToHashSet();

        Dictionary<string, float> scoreDict = new();

        Console.WriteLine("Possible outputs:");
        foreach (string output in noDupes)
        {
            float score = StringScorer.Score(output);
            if (score < Constants.scoreThreshold) continue;
            scoreDict[output] = score;
        }

        foreach (KeyValuePair<string, float> output in scoreDict.OrderBy(kv => kv.Value))
        {
            Console.WriteLine($"{output.Key} ({output.Value})");
        }
    }
}