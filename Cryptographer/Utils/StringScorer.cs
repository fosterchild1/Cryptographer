using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Cryptographer.Utils
{
    internal class StringScorer
    {
        public static string json = File.ReadAllText("resources/trigrams.json");
        public static string qjson = File.ReadAllText("resources/quadgrams.json");

        public static Dictionary<string, float>? trigrams = JsonSerializer.Deserialize<Dictionary<string, float>>(json);
        public static Dictionary<string, float>? quadgrams = JsonSerializer.Deserialize<Dictionary<string, float>>(qjson);

        public static float Score(string input, List<KeyValuePair<char, int>> analysis)
        {
            // to stop compiler from being an annoyance
            if (quadgrams == null || trigrams == null)
                return 0;

            // if it has less than 3 unique characters (and also check if there even is an analysis)
            if (analysis.Count > 0 && analysis.Count <= 3)
                return 0;

            string modifiedInput = ProjUtils.RemoveWhitespaces(input).ToUpper();
            int length = modifiedInput.Length;

            float quadScore = 0.0f;
            for (int i = 0; i <= length; i++)
            {
                // .TryGetValue is less performant, because most times the substr is not present in the dictionary
                string substr = modifiedInput.Substring(i, Math.Min(4, length - i));
                if (!quadgrams.ContainsKey(substr)) {
                    quadScore -= input.Length; // penalize by str length
                    continue; 
                }

                quadScore += quadgrams[substr] * input.Length / 3;
            }

            return quadScore;
        }
    }
}
