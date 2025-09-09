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

        private static string json = File.ReadAllText("resources/trigrams.json");
        private static string qjson = File.ReadAllText("resources/quadgrams.json");

        private static Dictionary<string, float>? trigrams = JsonSerializer.Deserialize<Dictionary<string, float>>(json, options);
        private static Dictionary<string, float>? quadgrams = JsonSerializer.Deserialize<Dictionary<string, float>>(qjson, options);

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