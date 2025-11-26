using Cryptographer.Classes;
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
        // PRUNING
        private HashSet<string> disallowedTwice = new() { "ROT-47", "Reverse", "Atbash", "Keyboard Substitution", "Caesar", "ASCII Shift", "Vigenère", "Beaufort" };

        private ConcurrentDictionary<string, bool> seenInputs = new();

        // WHATEVER
        public searchStatus status = searchStatus.NOT_STARTED;

        public PriorityQueue<DecryptionBranch, double> queue = new();

        public Stopwatch timer = new();

        // ACTUAL SEARCH
        public string key = "";

        public string input = "";

        public int totalDecryptions = 0;

        private void CheckPlaintext(string text, StringInfo info, DecryptionBranch branch)
        {
            StringType type = StringClassifier.Classify(text, info);
            if (type == StringType.GIBBERISH) return;

            bool isStopped = !timer.IsRunning; // avoid starting if its stopped

            timer.Stop();
            bool indeedPlaintext = PrintUtils.AskOutput(text, branch);
            if (indeedPlaintext)
            {
                Console.WriteLine($"String type: {type.ToString().ToLower()} | Took {Math.Round(timer.Elapsed.TotalMilliseconds / 1000, 3)} seconds.");
                status = searchStatus.SUCCESS;
                return;
            }

            if (!isStopped) timer.Start();
        }

        private void ExpandNode(DecryptionNode node, StringInfo info, bool fallback = false)
        {
            string nodeText = node.Text;
            string lastMethod = node.Method;
            bool failedAll = true;

            List<IDecoder> decoderList = (key != "" ? DecoderFactory.GetAll() : DecoderFactory.GetGeneric());

            foreach (IDecoder method in decoderList)
            {
                string methodName = method.Name;
                if (methodName == lastMethod && disallowedTwice.Contains(methodName)) continue;

                // if method is fallback but not fallback OR method doesnt require fallback and is fallback
                if (method.IsFallback ^ fallback != false) continue;

                double probability = method.CalculateProbability(nodeText, info);
                if (probability > 0.9) continue;

                if (probability < 0.7)
                    failedAll = false;

                queue.Enqueue(new(node, probability, method), probability);
            }

            // fallbacks
            if (!failedAll || fallback) return;
            ExpandNode(node, info, true);
        }

        private void ExpandBranch(DecryptionBranch branch)
        {
            DecryptionNode branchParent = branch.Parent;
            byte depth = branchParent.Depth;
            string parentText = branchParent.Text;

            StringInfo info = new(parentText);
            List<string> outputs = branch.Method.Decrypt(parentText, info, key);

            bool printed = false;
            bool failedAll = true;

            foreach (string output in outputs)
            {
                StringInfo newInfo = new(output);
                if (!StringClassifier.IsValid(output, parentText, newInfo, seenInputs)) continue;
                seenInputs.TryAdd(output, true);
                totalDecryptions++;

                if (Config.debug && !printed)
                {
                    PrintUtils.PrintDbgDecryption(branch);
                    printed = true;
                }

                // score it
                CheckPlaintext(output, newInfo, branch);

                DecryptionNode node = new(output, (byte)(depth + 1), branch.Method.Name, branchParent);

                failedAll = false;
                ExpandNode(node, newInfo);
            }

            if (!failedAll || branch.Method.IsFallback) return;
            ExpandNode(branchParent, info, true);
        }

        public void Search()
        {
            // the input could just be gibberish
            if (!StringClassifier.IsValid(input, "", new(input), new())) { status = searchStatus.FAILED; return; }
            status = searchStatus.SEARCHING;

            // use a priority queue alongside a CalculateProbability function
            // probabilities > 0.9 don't get checked
            DecryptionNode root = new(input, 1, "", new DecryptionNode());
            ExpandNode(root, new(input));

            // loop
            timer.Start();
            while (queue.TryDequeue(out DecryptionBranch? branch, out double _))
            {
                if (status != searchStatus.SEARCHING || timer.ElapsedMilliseconds / 1000 >= Config.searchTimeout)
                    break;

                // expand branch
                try
                {
                    DecryptionNode node = branch.Parent;
                    if (node.Depth > Config.maxDepth) continue;

                    ExpandBranch(branch);
                }
                catch (AggregateException exception)
                {
                    foreach (Exception ex in exception.InnerExceptions)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Task failed: {ex.Message}\n{ex.StackTrace!.Split("""--- End of stack""")[0]}");
                    }
                }

            }

            if (status == searchStatus.SEARCHING) status = searchStatus.FAILED; 
        }
        
        public Searcher(string inputIn, string keyIn)
        {

            input = inputIn;
            key = keyIn;
        }
    }
}
