using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryptographer.DecryptionUtils
{
    internal class StringScorer
    {
        public static string json = File.ReadAllText("trigrams.json");

        public static Dictionary<string, float>? trigrams = JsonSerializer.Deserialize<Dictionary<string, float>>(json);

        public static float Score(string input)
        {
            // to stop compiler from being an annoyance
            if (trigrams == null)
            {
                return 0;
            }

            string modifiedInput = input.Replace(" ", "").ToUpper();

            float totalScore = 0.0f;
            for (int i=0; i<= modifiedInput.Length; i++)
            {
                string substr = modifiedInput.Substring(i, Math.Min(3, modifiedInput.Length - i));
                if (!trigrams.TryGetValue(substr, out float score)) { continue; }

                totalScore += score;
            }

            return totalScore;
        }
    }
}
