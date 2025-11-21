using System.Collections.Concurrent;

namespace Cryptographer.Utils
{
    internal class StringScorer
    {
        // link stuff
        private static List<string> prefixes = new() { "https://", "http://" };
        private static HashSet<string> subdomains = new() { "ww.", "api", "dev", "docs", "store", "en", "fr", "wiki", "ro" };
        private static HashSet<string> topdomains = new() { "com", "net", "org", "tv", "fr", "en", "ro", "edu", "gov", "pro", "lol", "io", "co" };
        private static HashSet<string> extensions = new() { "htm", "html", "php", "css", "js", "json", "txt" };

        // CTF
        private static HashSet<string> CTFprefixes = new() { "flag", "ctf", "httb", "thm", "cftlearn", "picoctf", "dctf" };
        private static char[] CTFSymbols = { ':', '^', '-', '{' };

        // SEARCHER STUFF (makes more sense to be here tbh)
        public static bool IsValidDecryption(string output, string last, StringInfo info, ConcurrentDictionary<string, bool> seenInputs)
        {
            // aka useless string
            if (PrintUtils.RemoveWhitespaces(output).Length <= 3)
                return false;

            // TO NOT LOG IN CONSOLE
            if (seenInputs.ContainsKey(output))
                return false;

            // hasnt changed
            if (last == output)
                return false;

            // a single character
            if (info.uniqueCharacters < 2)
                return false;

            // anything that's under SPACE, created mostly by running base methods on non-base encrypted inputs
            char min = info.minChar;
            if (min < 32 && min != 10) // exclude line feed
                return false;

            // currently doesnt support base65536 or base2048 or those typa stuff
            if (info.maxChar > 127)
                return false;

            return true;
        }

        private static bool IsCTF(string input)
        {
            input = input.ToLower();

            int index = input.IndexOfAny(CTFSymbols);
            if (index == -1)
                return false;

            string split = input.AsSpan(0, index).ToString();
            if (split != null && CTFprefixes.Contains(split))
                return true;

            return false;
        }

        private static bool IsLink(string input)
        {
            input = input.ToLower();

            int score = 0;

            // prefixes (https://)
            foreach (string pref in prefixes)
            {
                if (!input.StartsWith(pref)) continue;

                score += 5;
                break;
            }

            input = input.Replace("https://", "").Replace("http://", ""); // ugly
            string[] split = input.Split(".");
            int splitLength = split.Length;

            // subdomains (www.)
            string subdomain = split[0];
            if (subdomains.Contains(subdomain))
                score += 2;

            // topdomains (.com)
            if ((splitLength >= 2 && topdomains.Contains(split[1])) || (splitLength >= 3 && topdomains.Contains(split[2])))
                score += 4;

            // extensions (.html)
            string extension = split[split.Length - 1];
            if (extensions.Contains(extension))
                score += 7;

            return score > 8;

        }

        private static float CalculateScoreForType(string input, int step, Dictionary<string, float> dict)
        {
            // how this works: we have a window of either 4 or 3 characters and we check for those in a dictionry
            // each string has a score, if it doesnt then it gets penalized proportional to the string length
            // if it has a score, add its score proportional to the string length
            // for smaller inputs, also keep track of seen strings and if we have seen those before we also penalize

            if (dict == null) return 0;

            int length = input.Length;
            float score = 0;

            Dictionary<string, short> seen = new();

            for (int i = 0; i < length; i++)
            {
                string substr = input.AsSpan(i, Math.Min(step, length - i)).ToString();

                float val = dict.GetValueOrDefault(substr);
                if (val == 0f)
                {
                    score -= length; // penalize by str length
                    continue;
                }

                short substrSeenAmount = (short)(seen.GetValueOrDefault(substr) + 1);

                seen[substr] = substrSeenAmount;
                score += val * length / substrSeenAmount / 4f; // fine tuned value that stops most non-plaintext strings
            }

            return score;
        }

        public static float Score(string input, StringInfo info)
        {
            // if it has less than 3 unique characters (and also check if there even is an analysis)
            var analysis = info.frequencyAnalysis;
            if (analysis.Count > 0 && analysis.Count <= 3)
                return 0;

            if (IsLink(input) || IsCTF(input))
            {
                return float.MaxValue;
            }

            string modifiedInput = PrintUtils.RemoveWhitespaces(input).ToUpper();

            bool tri = Config.useTrigrams || modifiedInput.Length <= 6;

            return CalculateScoreForType(modifiedInput, (tri ? 3 : 4), (tri ? Ngrams.trigrams! : Ngrams.quadgrams!));
        }
    }
}