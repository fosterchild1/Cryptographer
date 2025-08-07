using Cryptographer.DecryptionMethods;
using Cryptographer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

class DecryptionNode(string Text="", byte Depth=0, string Method="", DecryptionNode? Parent=null)
{
    public string Text = Text;
    public byte Depth = Depth;
    public string Method = Method;
    public DecryptionNode? Parent = Parent;
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

    public override string ToString()
    {
        return $"Text: {Text}, Depth: {Depth}, Method: {Method}";
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
            new Hexadecimal(),
            new Base32()
        };

        private static List<string> disallowedTwice = new() { "Reverse", "Atbash", "Keyboard Substitution" };
        // to not redo them
        private static HashSet<string> seenInputs = new();

        public static bool CheckOutput(string output, string input)
        {
            // aka useless string
            if (ProjUtils.RemoveWhitespaces(output).Length <= 3)
                return false;

            // TO NOT LOG IN CONSOLE
            if (seenInputs.Contains(output))
                return false;

            // hasnt changed
            if (input == output)
                return false;

            return true;
        }

        public static List<string> Search(string input)
        {
            // bfs, has many advantages over dfs

            DecryptionNode root = new(input, 1, "", new DecryptionNode());
            Queue<DecryptionNode> queue = new();
            queue.Enqueue(root); // Pretty ugly

            // loop
            while (queue.Count > 0)
            {
                DecryptionNode currentNode = queue.Dequeue();

                // conditions
                if (currentNode.Depth > Constants.maxDepth) continue;
                if (seenInputs.Contains(currentNode.Text) || currentNode.Parent == null) continue; // == null to stop annoyances
              
                string lastMethod = currentNode.Parent.Method;
                string newInput = currentNode.Text;

                List<KeyValuePair<char, int>> analysis = FrequencyAnalysis.AnalyzeFrequency(currentNode.Parent.Text);

                foreach (IDecryptionMethod method in methods)
                {
                    string methodName = method.Name;

                    // these dont make sense to do twice in a row
                    if (lastMethod == methodName && disallowedTwice.Contains(methodName)) continue;

                    List<string> outputs = method.Decrypt(newInput, analysis);

                    foreach (string output in outputs)
                    {
                        if (!CheckOutput(output, newInput)) continue;
                        seenInputs.Add(newInput);

                        if (StringScorer.Score(output, analysis) > Constants.scoreBreakSearchThreshold * input.Length)
                        {
                            Console.WriteLine($"Possible Output: {output}");
                            //break;
                        }

                        DecryptionNode newNode = new(output, (byte)(currentNode.Depth + 1), methodName, currentNode);
                        queue.Enqueue(newNode);
                        currentNode.Children.Add(newNode);
                    }
                }
            }

            List<string> results = new();
            root.GetLeaves(results);
            return results;
        }
    }
}
