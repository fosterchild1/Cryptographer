using Cryptographer.Classes;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryptographer.DecryptionMethods
{
    public class Polybius : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info, string key)
        {
            StringBuilder output = new();
            int length = input.Length;

            for (int i = 0; i < length; i+=2)
            {
                // idx = 5n1 - (5 - n2) - 1 = 5(n1 - 1) + n2 - 1
                string bigram = input.Substring(i, Math.Min(2, length - i));

                bool s = int.TryParse(bigram[0].ToString(), out int n1);
                bool s2 = int.TryParse(bigram[1].ToString(), out int n2);
                if (!s || !s2) return new();
   
                int keyIdx = 5 * (n1 - 1) + n2 - 1;
                output.Append(key[keyIdx]);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            if (input.Length % 2 != 0) return 1;

            // 1-9 only
            var analysis = info.frequencyAnalysis;
            if (analysis.Count <= 2 || analysis.Count >= 9) return 1;

            string withoutNum = Regex.Replace(input, "[0-9]", string.Empty);
            return withoutNum == string.Empty ? 0.15 : 1; // unlike others, this one doesnt have a seperator. so 0.15
        }

        public string Name { get { return "Polybius"; } }
        public KeyLevel RequiredKey { get { return KeyLevel.Keyed; } }
        public bool IsFallback { get { return false; } }
    }
}
