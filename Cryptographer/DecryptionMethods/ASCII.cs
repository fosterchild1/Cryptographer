using Cryptographer.Classes;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryptographer.DecryptionMethods
{
    public class ASCII : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            StringBuilder output = new();
            foreach (string s in input.Split(" "))
            {
                // handle double/triple/quadruple/... spaces between nums
                if (string.IsNullOrWhiteSpace(s)) continue;

                if (!int.TryParse(s, out int integer)) return new();
                if (integer <= 0 || integer > 127) return new();

                output.Append((char)integer);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;
            if (analysis.Count > 11) return 1; // it should only have 0-9 and a space.

            string withoutNum = Regex.Replace(input, "[0-9]", string.Empty);
            return string.IsNullOrWhiteSpace(withoutNum) ? 0.1 : 1;
        }

        public string Name { get { return "ASCII"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
