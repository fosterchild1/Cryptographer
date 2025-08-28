using Cryptographer.Classes;
using Cryptographer.DecryptionMethods;
using Cryptographer.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Cryptographer
{
    internal class Searcher
    {
        private List<IDecryptionMethod> methods = new()
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
            new Base32(),
            new Base85(),
            new ASCII(),
            new Octal(),
            new Baudot(),
            new Trilateral(),
        };

        private HashSet<string> disallowedTwice = new() { "Reverse", "Atbash", "Keyboard Substitution" };
        // to not redo them
        private ConcurrentDictionary<string, byte> seenInputs = new();

        public static bool success = false;

        public SearchQueue<DecryptionBranch, double> queue = new(Environment.ProcessorCount);
        private bool CheckOutput(string output, string input)
        {
            // aka useless string
            if (ProjUtils.RemoveWhitespaces(output).Length <= 3)
                return false;

            // TO NOT LOG IN CONSOLE
            if (seenInputs.ContainsKey(output))
                return false;

            // hasnt changed
            if (input == output)
                return false;

            return true;
        }

        private void ExpandNode(DecryptionNode node, List<KeyValuePair<char, int>> analysis)
        {
            string input = node.Text;
            string lastMethod = node.Method;

            foreach (IDecryptionMethod method in methods)
            {
                string methodName = method.Name;
                if (methodName == lastMethod && disallowedTwice.Contains(methodName)) continue;

                double probability = method.CalculateProbability(input, analysis);
                if (probability > 0.9) continue;

                queue.Enqueue(new(node, probability, method), probability);
            }
        }

        private void ExpandBranch(DecryptionBranch branch)
        {
            DecryptionNode branchParent = branch.Parent;
            byte depth = branchParent.Depth;
            string parentText = branchParent.Text;

            List<KeyValuePair<char, int>> analysis = FrequencyAnalysis.AnalyzeFrequency(parentText);

            List<string> outputs = branch.Method.Decrypt(parentText, analysis);
            foreach (string output in outputs)
            {
                if (!CheckOutput(output, parentText)) continue;
                seenInputs.TryAdd(output, 0);

                // TEMP
                List<KeyValuePair<char, int>> newAnalysis = FrequencyAnalysis.AnalyzeFrequency(output);
                if (StringScorer.Score(output, newAnalysis) > Constants.scorePrintThreshold)
                {
                    Console.WriteLine($"Possible Output: {output}");
                    success = true;
                }
                DecryptionNode node = new(output, (byte)(depth + 1), branch.Method.Name, branchParent);
                ExpandNode(node, newAnalysis);
            }
        }
        
        public void Search(string input)
        {
            // use a priority queue alongside a CalculateProbability function
            // probabilities > 0.9 don't get checked
            DecryptionNode root = new(input, 1, "", new DecryptionNode());
            ExpandNode(root, FrequencyAnalysis.AnalyzeFrequency(input));

            int workers = Environment.ProcessorCount;

            int active = 0;
            Task[] tasks = new Task[workers];

            // loop
            for (int i = 0; i < workers; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    while (true)
                    {
                        // get next batch
                        if (!queue.TryDequeue(out DecryptionBranch branch, out double _))
                        {
                            if (queue.IsEmpty() && Volatile.Read(ref active) == 0) break;
                            Thread.SpinWait(64);
                            continue;
                        }

                        // expand batch
                        Interlocked.Increment(ref active);
                        try
                        {
                            DecryptionNode node = branch.Parent;
                            if (node.Depth > Constants.maxDepth) continue;

                            ExpandBranch(branch);
                        }
                        finally
                        {
                            Interlocked.Decrement(ref active);
                        }

                    }
                });
            }

            // wait for all to finish
            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException ae)
            {
                foreach (Exception ex in ae.InnerExceptions)
                {
                    Console.WriteLine("Task failed: " + ex);
                }
                throw;
            }
        }
    }
}
