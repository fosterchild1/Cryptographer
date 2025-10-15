using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Cryptographer.Utils
{
    #region Ngrams
    public static class Ngrams
    {
        private static JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };

        private static string json = File.ReadAllText($"{AppContext.BaseDirectory}/resources/trigrams.json");
        public static string qjson = File.ReadAllText($"{AppContext.BaseDirectory}/resources/quadgrams.json");

        public static Dictionary<string, float>? trigrams;

        public static Dictionary<string, float>? quadgrams;

        public static void Wake()
        {
            trigrams = JsonSerializer.Deserialize<Dictionary<string, float>>(json, options);
            quadgrams = JsonSerializer.Deserialize<Dictionary<string, float>>(qjson, options);
        } // init
    }
    #endregion

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

            HashSet<string> seen = new();

            for (int i = 0; i < length; i++)
            {
                string substr = input.AsSpan(i, Math.Min(step, length - i)).ToString();

                float val = dict.GetValueOrDefault(substr);
                if (val == 0f || (seen.Contains(substr) && length <= 40))
                {
                    score -= length; // penalize by str length
                    continue;
                }

                seen.Add(substr);
                score += val * length / 4f; // fine tuned value that stops most non-plaintext strings
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