using Cryptographer.Classes;
using Cryptographer.DecryptionMethods;
using Cryptographer.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Cryptographer
{
    internal class Searcher
    {
        // METHODS
        private List<IDecryptionMethod> methods = new()
        {
            new Base64(), new Morse(), new Baconian(), new Binary(), new TapCode(), 
            new DNA(), new Hexadecimal(), new Base32(), new Base85(), new Base62(), 
            new Octal(), new Baudot(), new Trilateral(), new ROT47(), new uuencoding(),
            new A1Z26(), new ASCII()
        };

        private List<IDecryptionMethod> fallbackMethods = new()
        {
            new Caesar(), new KeyboardSubstitution(), new Atbash(), new Reverse()
        };

        // OTHER
        private HashSet<string> disallowedTwice = new() { "Reverse", "Atbash", "Keyboard Substitution", "Caesar" };
        // to not redo them
        private ConcurrentDictionary<string, byte> seenInputs = new();

        public static bool success = false;

        public SearchQueue<DecryptionBranch, double> queue = new(Constants.threadCount);

        public Stopwatch timer = new();

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

        private void ExpandNode(DecryptionNode node, List<KeyValuePair<char, int>> analysis, int workerIndex, bool fallback = false)
        {
            string input = node.Text;
            string lastMethod = node.Method;

            List<IDecryptionMethod> chosen = (fallback ? fallbackMethods : methods);

            foreach (IDecryptionMethod method in chosen)
            {
                string methodName = method.Name;
                if (methodName == lastMethod && disallowedTwice.Contains(methodName)) continue;

                double probability = method.CalculateProbability(input, analysis);
                if (probability > 0.9) continue;

                queue.Enqueue(new(node, probability, method), probability, workerIndex);
            }

        }

        private void ExpandBranch(DecryptionBranch branch, int workerIndex)
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
                    bool isStopped = !timer.IsRunning; // avoid starting if its stopped

                    timer.Stop();
                    bool plaintext = ProjUtils.AskOutput(output);
                    if (plaintext) {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"Took {Math.Round(timer.Elapsed.TotalMilliseconds / 1000, 3)} seconds.");
                        success = true; 
                        break; 
                    }

                    if (!isStopped) timer.Start();

                }
                DecryptionNode node = new(output, (byte)(depth + 1), branch.Method.Name, branchParent);
                ExpandNode(node, newAnalysis, workerIndex);
            }

            bool peeked = queue.TryPeek(out DecryptionBranch _, out double priority, workerIndex);
            if (peeked && priority <= 0.7) return;
            ExpandNode(branchParent, analysis, workerIndex, true); // run fallbacks
        }

        public void Search(string input)
        {
            // use a priority queue alongside a CalculateProbability function
            // probabilities > 0.9 don't get checked
            DecryptionNode root = new(input, 1, "", new DecryptionNode());
            ExpandNode(root, FrequencyAnalysis.AnalyzeFrequency(input), 0);
            int workers = Constants.threadCount;

            int active = 0;
            Task[] tasks = new Task[workers];

            // loop
            timer.Start();
            for (int i = 0; i < workers; i++)
            {
                int index = i;
                tasks[i] = Task.Run(() =>
                {
                    int workerIndex = index; // needed....... sadly
                    while (true)
                    {
                        // get next batch
                        if (!queue.TryDequeue(out DecryptionBranch branch, out double _, workerIndex) || success)
                        {
                            if ((queue.IsEmpty() && Volatile.Read(ref active) == 0) || success) break;
                            Thread.SpinWait(64);
                            continue;
                        }

                        // expand batch
                        Interlocked.Increment(ref active);
                        try
                        {
                            DecryptionNode node = branch.Parent;
                            if (node.Depth > Constants.maxDepth) continue;

                            ExpandBranch(branch, workerIndex);
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
