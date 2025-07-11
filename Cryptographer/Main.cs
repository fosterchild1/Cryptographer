using Cryptographer.DecryptionMethods;
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

    public int GetMaxDepth(int max = 0)
    {
        if (Children.Count == 0)
        {
            return Math.Max(Depth, max);
        }

        foreach (DecryptionNode child in Children)
        {
            max = child.GetMaxDepth(max);
        }

        return max;
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
        new Binary(),
    };

    private static int maxDepth = 0;

    // to not redo them
    private static Dictionary<string, DecryptionNode> memo = new();
    private static Dictionary<string, int> seenInputs = new();

    public static bool CheckOutput(string output, int depth = 1)
    {
        // aka useless string
        if (string.IsNullOrWhiteSpace(output))
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
  
        // memo cache
        if (memo.TryGetValue(input, out DecryptionNode? memoNode))
        {
            int maxDepth = memoNode.GetMaxDepth();
            int diff = Math.Max(maxDepth - memoNode.Depth, 1);

            List<string> leaves = new List<string>();
            memoNode.GetLeaves(leaves);

            foreach (string leaf in leaves)
            {
                DecryptionNode child = GetDecrypted(leaf, depth + diff);
                node.Children.Add(child);
            }

            return node;
        }


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
                if (StringScorer.Score(output) > 100)
                {
                    Console.WriteLine(output);
                    break;
                }


                DecryptionNode child = GetDecrypted(output, depth + 1, methodName);
                node.Children.Add(child);
            }
        }

        memo.TryAdd(input, node);
        return node;
    }

    static void Main(string[] args)
    {
        Console.WriteLine("cryptographer is a tool for decrypting text");

        string input = ProjUtils.GetInput();
        Console.Clear();
        maxDepth = ProjUtils.GetDepth();
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