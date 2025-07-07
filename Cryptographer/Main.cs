using Cryptographer.DecryptionMethods;
using Cryptographer.DecryptionUtils;
using Cryptographer.Utils;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Reflection;

class DecryptionNode
{
    public string Text;
    public int Depth;
    public string Method;
    public List<DecryptionNode> Children = new();

    public DecryptionNode(string text, int depth, string method)
    {
        Text = text;
        Depth = depth;
        Method = method;
    }

    public void GetLeaves(List<string> results)
    {
        if (Children.Count == 0)
        {
            results.Add(Text);
            return;
        }

        foreach (DecryptionNode child in Children)
        {
            child.GetLeaves(results);
        }
    }
}

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

    // to not redo them
    private static Dictionary<string, int> seenInputs = new();

    public static bool CheckOutput(string output, int depth = 1)
    {
        // aka useless string
        if (output.Replace(" ", "").Length <= 3)
            return false;

        bool found = seenInputs.TryGetValue(output, out int seenDepth);
        if (found && seenDepth <= depth)
            return false;

        return true;
    }

    public static DecryptionNode GetDecrypted(string input, int depth = 1, string lastMethod = "")
    {
        // dont go past max depth
        if (depth > maxDepth)
        {
            return new DecryptionNode(input, depth, lastMethod);
        }

        DecryptionNode node = new DecryptionNode(input, depth, lastMethod);

        List<KeyValuePair<char, int>> analysis = FrequencyAnalysis.AnalyzeFrequency(input);

        foreach (IDecryptionMethod method in methods)
        {
            string methodName = method.Name;

            // these dont make sense to do twice in a row
            if (lastMethod == methodName && (methodName == "Reverse" || methodName == "Atbash" || methodName == "Keyboard Substitution"))
            {
                continue;
            }

            List<string> outputs = method.Decrypt(input, analysis);

            foreach (string output in outputs)
            {
                if (!CheckOutput(output, depth)) continue;
                seenInputs.TryAdd(output, depth);

                DecryptionNode child = GetDecrypted(output, depth + 1, methodName);
                node.Children.Add(child);
            }
        }

        return node;
    }

    static void Main(string[] args)
    {
        Console.WriteLine("cryptographer is a tool for decrypting text");

        string input = Utils.GetInput();
        Console.Clear();
        maxDepth = Utils.GetDepth();
        Console.Clear();
        Console.WriteLine("Working...");

        DecryptionNode root = GetDecrypted(input, 1);

        List<string> outputs = new List<string>();
        root.GetLeaves(outputs);

        Dictionary<string, float> scoreDict = new();

        foreach (string output in outputs)
        {
            float score = StringScorer.Score(output);
            if (score < Constants.scoreThreshold) continue;
            scoreDict[output] = score;
        }

        Console.WriteLine("Possible outputs:");
        foreach (KeyValuePair<string, float> output in scoreDict.OrderBy(kv => kv.Value))
        {
            Console.WriteLine($"{output.Key} ({output.Value})");
        }
    }
}