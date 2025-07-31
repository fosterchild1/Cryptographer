using Cryptographer.DecryptionMethods;
using Cryptographer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

class DecryptionNode(string Text, byte Depth, string Method)
{
    public string Text = Text;
    public byte Depth = Depth;
    public string Method = Method;
    public List<DecryptionNode> Children = new();

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

namespace Cryptographer
{
    internal class Searcher
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

        public static DecryptionNode Search(string input, byte depth = 1, string lastMethod = "")
        {
            // dont go past max depth
            if (depth > Constants.maxDepth)
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
                    DecryptionNode child = Search(leaf, (byte)(depth + diff));
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

                    DecryptionNode child = Search(output, (byte)(depth + 1), methodName);
                    node.Children.Add(child);
                }
            }

            if (memo.TryGetValue(input, out DecryptionNode? val) && val.GetMaxDepth() > depth)
                memo[input] = node;
            else
                memo.Add(input, node);

            return node;
        }

    }
}
