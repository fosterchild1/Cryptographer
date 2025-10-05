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
            new A1Z26(), new ASCII(), new Brainfuck(), new Base58(), new Base45()
        };

        private List<IDecryptionMethod> fallbackMethods = new()
        {
            new Caesar(), new KeyboardSubstitution(), new Atbash(), new Reverse(), new ASCIIShift(), new Scytale()
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
            if (PrintUtils.RemoveWhitespaces(output).Length <= 3)
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

            // anything that's under SPACE, created mostly by running base methods on non-base encrypted inputs
            if (info.minChar < 32)
                return false;

            // currently doesnt support base65536 or base2048 or those typa stuff
            if (info.maxChar > 127)
                return false;

            return true;
        }

        private void CheckPlaintext(string text, StringInfo info, DecryptionBranch branch)
        {
            if (!(StringScorer.Score(text, info) > Config.scorePrintThreshold)) return;
            bool isStopped = !timer.IsRunning; // avoid starting if its stopped

            timer.Stop();
            bool plaintext = PrintUtils.AskOutput(text, branch);
            if (plaintext)
            {
                Console.WriteLine($"Took {Math.Round(timer.Elapsed.TotalMilliseconds / 1000, 3)} seconds.");
                status = searchStatus.SUCCESS;
                return;
            }

            if (!isStopped) timer.Start();
        }

        private void ExpandNode(DecryptionNode node, StringInfo info, int workerIndex, bool fallback = false)
        {
            string input = node.Text;
            string lastMethod = node.Method;

            bool failedAll = true;

            List<IDecryptionMethod> chosen = (fallback ? fallbackMethods : methods);

            foreach (IDecryptionMethod method in chosen)
            {
                string methodName = method.Name;
                if (methodName == lastMethod && disallowedTwice.Contains(methodName)) continue;

                double probability = method.CalculateProbability(input, info);
                if (probability > 0.9) continue;

                if (probability < 0.7)
                    failedAll = false;

                queue.Enqueue(new(node, probability, method), probability, workerIndex);
            }

            // fallbacks
            if (!failedAll) return;
            ExpandNode(node, info, workerIndex, true);
        }

        private void ExpandBranch(DecryptionBranch branch, int workerIndex)
        {
            DecryptionNode branchParent = branch.Parent;
            byte depth = branchParent.Depth;
            string parentText = branchParent.Text;

            StringInfo info = new(parentText);
            List<string> outputs = branch.Method.Decrypt(parentText, info);

            bool printedDebug = false;

            foreach (string output in outputs)
            {
                StringInfo newInfo = new(output);
                if (!CheckOutput(output, parentText, newInfo)) continue;
                seenInputs.TryAdd(output, 0);

                if (Config.debug && !printedDebug)
                {
                    printedDebug = true;
                    PrintUtils.PrintDbgDecryption(branch, workerIndex);
                }

                // score it
                CheckPlaintext(output, newInfo, branch);

                DecryptionNode node = new(output, (byte)(depth + 1), branch.Method.Name, branchParent);

                ExpandNode(node, newInfo, workerIndex);
            }
        }

        public void Search(string input)
        {
            if (!CheckOutput(input, "", new(input))) { status = searchStatus.FAILED; return; }
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
                        // get next branch and decide if should break
                        if (!queue.TryDequeue(out DecryptionBranch branch, out double _, workerIndex))
                        {
                            if ((queue.IsEmpty() && Volatile.Read(ref active) == 0)) break;
                            Thread.Sleep(64);
                            continue;
                        }

                        if (status != searchStatus.SEARCHING || timer.ElapsedMilliseconds / 1000 >= Config.searchTimeout)
                            break;

                        // expand branch
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
                    Console.WriteLine($"Task failed: {ex.Message}\n{ex.StackTrace!.Split("""--- End of stack""")[0]}");
                }
                throw;
            }

            if (status == searchStatus.SEARCHING) status = searchStatus.FAILED; 
        }
    }
}
