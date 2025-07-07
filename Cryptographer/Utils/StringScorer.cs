using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Cryptographer.DecryptionUtils
{
    internal class StringScorer
    {
        public static string json = File.ReadAllText("trigrams.json");
        public static string qjson = File.ReadAllText("quadgrams.json");

        public static Dictionary<string, float>? trigrams = JsonSerializer.Deserialize<Dictionary<string, float>>(json);
        public static Dictionary<string, float>? quadgrams = JsonSerializer.Deserialize<Dictionary<string, float>>(qjson);

        public static float Score(string input)
        {
            // to stop compiler from being an annoyance
            if (quadgrams == null || trigrams == null)
            {
                return 0;
            }

            string modifiedInput = input.Replace(" ", "").ToUpper();

            float quadScore = 0.0f;
            for (int i=0; i<= modifiedInput.Length; i++)
            {
                string substr = modifiedInput.Substring(i, Math.Min(4, modifiedInput.Length - i));
                if (!quadgrams.TryGetValue(substr, out float score)) {
                    quadScore -= 2;
                    continue; 
                }

                quadScore += score;
            }

            return quadScore;
        }
    }
}
