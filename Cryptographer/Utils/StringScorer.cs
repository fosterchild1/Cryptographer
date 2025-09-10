using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Cryptographer.Utils
{
    internal class StringScorer
    {
        private static JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };

        private static string json = File.ReadAllText($"{AppContext.BaseDirectory}/resources/trigrams.json");
        private static string qjson = File.ReadAllText($"{AppContext.BaseDirectory}resources/quadgrams.json");

        private static Dictionary<string, float>? trigrams = JsonSerializer.Deserialize<Dictionary<string, float>>(json, options);
        private static Dictionary<string, float>? quadgrams = JsonSerializer.Deserialize<Dictionary<string, float>>(qjson, options);

        private static  List<string> prefixes = new() { "https://", "http://" };
        private static HashSet<string> subdomains = new() { "www.", "api.", "dev.", "docs.", "store.", "en.", "fr.", "wiki." };
        private static HashSet<string> topdomains = new() { ".com", ".net", ".org", ".tv", ".fr", ".en", ".edu", ".gov", ".pro", ".lol", ".io" };
        private static HashSet<string> extensions = new() { ".htm", ".html", ".php", ".css", ".js", ".json" };


        private static bool IsLink(string input)
        {
            int score = 0;

            // prefixes
            foreach (string pref in prefixes)
            {
                if (!input.StartsWith(pref)) continue;

                score += 5;
                break;
            }

            input = input.Replace("https://", "").Replace("http://", "");
            string[] split = input.Split(".");
            int splitLength = split.Length;

            // subdomains
            string subdomain = split[0];
            if (subdomains.Contains(subdomain))
                score += 2;

            // topdomains
            if ((splitLength >= 2 && topdomains.Contains(split[1])) || (splitLength >= 3 && topdomains.Contains(split[2])))
                score += 4;

            // extensions
            string extension = split[split.Length - 1];
            if (extensions.Contains(extension))
                score += 7;

            return score > 8;

        }

        public static float Score(string input, List<KeyValuePair<char, int>> analysis)
        {
            // to stop compiler from being an annoyance
            if (quadgrams == null || trigrams == null)
                return 0;

            // if it has less than 3 unique characters (and also check if there even is an analysis)
            if (analysis.Count > 0 && analysis.Count <= 3)
                return 0;

            // is link?
            if (IsLink(input))
                return float.MaxValue;

            string modifiedInput = ProjUtils.RemoveWhitespaces(input).ToUpper();
            int length = modifiedInput.Length;

            float quadScore = 0.0f;
            for (int i = 0; i <= length; i++)
            {
                string substr = modifiedInput.AsSpan(i, Math.Min(4, length - i)).ToString();

                float val = quadgrams.GetValueOrDefault(substr);
                if (val == 0f) {
                    quadScore -= length; // penalize by str length
                    continue; 
                }

                quadScore += val * length / 3.5f;
            }

            return quadScore;
        }
    }
}