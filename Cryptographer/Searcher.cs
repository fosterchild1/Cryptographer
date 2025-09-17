using Cryptographer.Classes;
using Cryptographer.DecryptionMethods;
using Cryptographer.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;

enum searchStatus
{
    NOT_STARTED = 0,
    SEARCHING = 1,
    SUCCESS = 2,
    FAILED = 3,
}

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
            new A1Z26(), new ASCII(), new Brainfuck()
        };

        private List<IDecryptionMethod> fallbackMethods = new()
        {
            new Caesar(), new KeyboardSubstitution(), new Atbash(), new Reverse(), new ASCIIShift()
        };

        // OTHER
        private HashSet<string> disallowedTwice = new() { "ROT-47", "Reverse", "Atbash", "Keyboard Substitution", "Caesar", "ASCII Shift" };
        // to not redo them
        private ConcurrentDictionary<string, byte> seenInputs = new();

        public searchStatus status = searchStatus.NOT_STARTED;

        public SearchQueue<DecryptionBranch, double> queue = new(Config.threadCount);

        public Stopwatch timer = new();

        private bool CheckOutput(string output, string input, StringInfo info)
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

            // a single character
            if (info.uniqueCharacters < 2)
                return false;

            // anything thats under SPACE
            if (info.minChar < 32)
                return false;

            return true;
        }

        private void ExpandNode(DecryptionNode node, StringInfo info, int workerIndex, bool fallback = false)
        {
            string input = node.Text;
            string lastMethod = node.Method;

            List<IDecryptionMethod> chosen = (fallback ? fallbackMethods : methods);

            foreach (IDecryptionMethod method in chosen)
            {
                string methodName = method.Name;
                if (methodName == lastMethod && disallowedTwice.Contains(methodName)) continue;

                double probability = method.CalculateProbability(input, info);
                if (probability > 0.9) continue;

                queue.Enqueue(new(node, probability, method), probability, workerIndex);
            }

        }

        private void ExpandBranch(DecryptionBranch branch, int workerIndex)
        {
            DecryptionNode branchParent = branch.Parent;
            byte depth = branchParent.Depth;
            string parentText = branchParent.Text;

            StringInfo info = new(parentText);
            List<string> outputs = branch.Method.Decrypt(parentText, info);

            foreach (string output in outputs)
            {
                if (!CheckOutput(output, parentText, info)) continue;
                seenInputs.TryAdd(output, 0);

                // TEMP
                StringInfo newInfo = new(output);
                if (StringScorer.Score(output, newInfo) > Config.scorePrintThreshold)
                {
                    bool isStopped = !timer.IsRunning; // avoid starting if its stopped

                    timer.Stop();
                    bool plaintext = ProjUtils.AskOutput(output, branch);
                    if (plaintext) 
                    {
                        Console.WriteLine($"Took {Math.Round(timer.Elapsed.TotalMilliseconds / 1000, 3)} seconds.");
                        status = searchStatus.SUCCESS; 
                        break; 
                    }

                    if (!isStopped) timer.Start();

                }
                DecryptionNode node = new(output, (byte)(depth + 1), branch.Method.Name, branchParent);
                ExpandNode(node, newInfo, workerIndex);
            }

            bool peeked = queue.TryPeek(out DecryptionBranch _, out double priority, workerIndex);
            if (peeked && priority <= 0.7) return;
            ExpandNode(branchParent, info, workerIndex, true); // run fallbacks
        }

        public void Search(string input)
        {
            if (!CheckOutput(input, "", new(input))) return;
            status = searchStatus.SEARCHING;

            // use a priority queue alongside a CalculateProbability function
            // probabilities > 0.9 don't get checked
            DecryptionNode root = new(input, 1, "", new DecryptionNode());
            ExpandNode(root, new(input), 0);
            int workers = Config.threadCount;

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
                        if (!queue.TryDequeue(out DecryptionBranch branch, out double _, workerIndex) || status != searchStatus.SEARCHING)
                        {
                            if ((queue.IsEmpty() && Volatile.Read(ref active) == 0) || status != searchStatus.SEARCHING) break;
                            Thread.SpinWait(64);
                            continue;
                        }

                        // expand batch
                        Interlocked.Increment(ref active);
                        try
                        {
                            DecryptionNode node = branch.Parent;
                            if (node.Depth > Config.maxDepth) continue;

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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Task failed: {ex.StackTrace!.Split("""--- End of stack""")[0]}");
                }
                throw;
            }

            if (status == searchStatus.SEARCHING) status = searchStatus.FAILED; 
        }
    }
}
