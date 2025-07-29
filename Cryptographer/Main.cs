using Cryptographer.DecryptionMethods;
using Cryptographer.Utils;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;

class DecryptionNode
{
    public string Text;
    public byte Depth; // i dont see why youd need to go further than 255.
    public string Method;
    public List<DecryptionNode> Children = new();

    public DecryptionNode(string text, byte depth, string method)
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

    public byte GetMaxDepth(byte max = 0)
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
        new TapCode(),
        new DNA(),
    };

    private static byte maxDepth = 0;

    // to not redo them
    private static Dictionary<string, DecryptionNode> memo = new();
    private static Dictionary<string, int> seenInputs = new();

    public static bool CheckOutput(string output, string input, byte depth = 1)
    {
        // aka useless string
        if (string.IsNullOrWhiteSpace(output) || ProjUtils.RemoveWhitespaces(output).Length <= 3)
            return false;

        // seen somewhere higher up the tree
        bool found = seenInputs.TryGetValue(output, out int seenDepth);
        if (found && seenDepth <= depth)
            return false;

        if (input == output)
            return false;

        return true;
    }

    public static DecryptionNode GetDecrypted(string input, byte depth = 1, string lastMethod = "")
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
            byte maxDepth = memoNode.GetMaxDepth();
            byte diff = (byte)Math.Max(maxDepth - memoNode.Depth, 1);

            List<string> leaves = new List<string>();
            memoNode.GetLeaves(leaves);

            foreach (string leaf in leaves)
            {
                DecryptionNode child = GetDecrypted(leaf, (byte)(depth + diff));
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
                if (!CheckOutput(output, input, depth)) continue;
                seenInputs.TryAdd(output, depth);

                if (StringScorer.Score(output, analysis) > Constants.scoreBreakSearchThreshold)
                {
                    Console.WriteLine($"Possible Output: {output}");
                    break;
                }
 
                DecryptionNode child = GetDecrypted(output, (byte)(depth + 1), methodName);
                node.Children.Add(child);
            }
        }

        if (memo.TryGetValue(input, out DecryptionNode? val) && val.GetMaxDepth() > depth)
            memo[input] = node;
        else
            memo.Add(input, node);

        return node;
    }

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

        if (!byte.TryParse(argDict.GetValueOrDefault("maxdepth"), out maxDepth))
            maxDepth = ProjUtils.GetDepth();
        Console.Clear();


        Console.WriteLine("Working...");

        DecryptionNode root = GetDecrypted(input, 1);

        List<string> outputs = new List<string>();
        root.GetLeaves(outputs);

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