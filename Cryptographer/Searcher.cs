using Cryptographer.Classes;
using Cryptographer.DecryptionMethods;
using Cryptographer.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            new Base32(),
            new ASCII(),
            new Octal(),
            new Baudot(),
            new Trilateral(),
        };

        private static List<string> disallowedTwice = new() { "Reverse", "Atbash", "Keyboard Substitution" };
        // to not redo them
        private static ConcurrentDictionary<string, byte> seenInputs = new();

        public static bool success = false;

        public static bool CheckOutput(string output, string input)
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

        private static void SearchMethods(SearchQueue<DecryptionNode, double> queue, DecryptionNode currentNode)
        {
            string lastMethod = currentNode.Parent?.Method ?? "";
            string input = currentNode.Text;

            List<KeyValuePair<char, int>> analysis = FrequencyAnalysis.AnalyzeFrequency(input);

            foreach (IDecryptionMethod method in methods)
            {
                string methodName = method.Name;

                // these dont make sense to do twice in a row
                if (lastMethod == methodName && disallowedTwice.Contains(methodName)) continue;

                // check probability
                double probability = method.CalculateProbability(input, analysis);
                if (probability >= 0.9) continue;

                List<string> outputs = method.Decrypt(input, analysis);

                foreach (string output in outputs)
                {
                    if (!CheckOutput(output, input)) continue;

                    // TEMP
                    List<KeyValuePair<char, int>> newAnalysis = FrequencyAnalysis.AnalyzeFrequency(output);
                    if (StringScorer.Score(output, newAnalysis) > Constants.scorePrintThreshold)
                    {
                        Console.WriteLine($"Possible Output: {output}");
                        success = true;
                    }

                    DecryptionNode newNode = new(output, (byte)(currentNode.Depth + 1), methodName, currentNode);

                    queue.Enqueue(newNode, probability);
                    currentNode.Children.Add(newNode);
                }
            }
        }

        public static void Search(string input)
        {
            // use a priority queue alongside a CalculateProbability function
            // probabilities > 0.9 don't get checked

            DecryptionNode root = new(input, 1, "", new DecryptionNode());

            int workers = Environment.ProcessorCount;
            SearchQueue<DecryptionNode, double> queue = new(workers);
            queue.Enqueue(root, 0);

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
                        if (!queue.TryDequeueBatch(out var batch))
                        {
                            if (queue.IsEmpty() && Volatile.Read(ref active) == 0) break;
                            Thread.SpinWait(64);
                            continue;
                        }

                        // expand batch
                        Interlocked.Increment(ref active);
                        try
                        {
                            foreach (var (node, _) in batch)
                            {
                                if (node == null) continue;
                                if (seenInputs.ContainsKey(node.Text) || node.Depth > Constants.maxDepth) continue;
                                seenInputs.TryAdd(node.Text, 0);

                                SearchMethods(queue, node);
                            }
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
                foreach (var ex in ae.InnerExceptions)
                {
                    Console.WriteLine("Task failed: " + ex);
                }
                throw;
            }
        }
    }
}
