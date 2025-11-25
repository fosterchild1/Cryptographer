using System.Collections.Concurrent;
using System.Text.RegularExpressions;

enum StringType
{
    GIBBERISH = 0,
    PLAINTEXT = 1,
    LINK = 2,
    CTF_FLAG = 3,
}

namespace Cryptographer.Utils
{
    internal class StringClassifier
    {
        // link stuff
        private static readonly List<string> prefixes = new() { "https://", "http://" };
        private static readonly HashSet<string> subdomains = new() { "www", "api", "dev", "docs", "store", "en", "fr", "wiki", "ro" };
        private static readonly HashSet<string> topdomains = new() { "com", "net", "org", "tv", "fr", "en", "ro", "edu", "gov", "pro", "lol", "io", "co", "dev" };
        private static readonly HashSet<string> extensions = new() { "htm", "html", "php", "css", "js", "json", "txt" };

        // CTF
        private static readonly HashSet<string> CTFprefixes = new() { "flag", "ctf", "httb", "thm", "cftlearn", "picoctf", "dctf" };
        private static readonly char[] CTFSymbols = { ':', '^', '-', '{' };

        // SEARCHER STUFF (makes more sense to be here tbh)
        public static bool IsValid(string output, string last, StringInfo info, ConcurrentDictionary<string, bool> seenInputs)
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

            string split = input.Substring(0, index);
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
            string[] split = Regex.Split(input, "[./]");
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

        private static float CalculateScoreForDict(string input, int step, Dictionary<string, float> dict)
        {
            // how this works: we have a window of either 4 or 3 characters and we check for those in a dictionry
            // each string has a score, if it doesnt then it gets penalized proportional to the string length
            // if it has a score, add its score proportional to the string length
            // we also keep track of seen strings and if we have seen those before we also penalize
            if (dict == null) return 0;

            int length = input.Length;
            float score = 0;

            Dictionary<string, short> seen = new();

            for (int i = 0; i < length; i++)
            {
                string substr = input.Substring(i, Math.Min(step, length - i));

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

        public static StringType Classify(string input, StringInfo info)
        {
            if (info.uniqueCharacters > 0 && info.uniqueCharacters <= 3)
                return 0;

            if (IsLink(input))
                return StringType.LINK;
            else if (IsCTF(input))
                return StringType.CTF_FLAG;

            string modifiedInput = PrintUtils.RemoveWhitespaces(input).ToUpper();

            bool tri = Config.useTrigrams || modifiedInput.Length <= 6;

            float score = CalculateScoreForDict(modifiedInput, (tri ? 3 : 4), (tri ? Ngrams.trigrams! : Ngrams.quadgrams!));

            return (score >= Config.scorePrintThreshold ? StringType.PLAINTEXT : StringType.GIBBERISH);
        }
    }
}