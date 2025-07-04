using Cryptographer;
using Cryptographer.DecryptionMethods;
using Cryptographer.DecryptionUtils;
using System.Diagnostics;

class Program
{

    private static List<IDecryptionMethod> methods = new()
    {
        new Reverse(),
        new Atbash(),
        new Base64(),
    };

    private static List<IListDecryptionMethod> listMethods = new()
    {
        new Morse(),
        new Baconian(),
        new KeyboardSubstitution()
    };

    private static int maxDepth = 0;

    public static List<string> GetDecrypted(string input, int depth = 1, string lastMethod = "")
    {
        if (depth > maxDepth)
            return new List<string> { input };

        List<KeyValuePair<char, int>> analysis = FrequencyAnalysis.AnalyzeFrequency(input);
        List<string> decrypted = new List<string>();

        // single output methods
        foreach (IDecryptionMethod method in methods)
        {
            if (lastMethod == method.Name && (method.Name == "Reverse" || method.Name == "Atbash"))
                continue;

            Utils.StartTimer(method.Name);
            string output = method.Decrypt(input, analysis);
            Console.WriteLine($"{method.Name}: {Utils.StopTimer(method.Name)}");
            if (output.Replace(" ", "").Length <= 2)
                continue;

            decrypted.AddRange(GetDecrypted(output, depth + 1, method.Name));
        }

        // multiple output methods
        foreach (IListDecryptionMethod method in listMethods)
        {
           // Utils.StartTimer(method.Name);
            List<string> outputs = method.Decrypt(input, analysis);
           // Console.WriteLine($"{method.Name}: {Utils.StopTimer(method.Name)}");
            foreach (string output in outputs)
            {
                decrypted.AddRange(GetDecrypted(output, depth + 1, method.Name));
            }
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

        Console.WriteLine("Possible outputs:");
        foreach (string output in noDupes)
        {
            float score = StringScorer.Score(output);
            Console.WriteLine($"{output} ({score})");
        }
    }
}