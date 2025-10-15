using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class DNA : IDecryptionMethod
    {
        private Dictionary<string, string> DNADictionary = MethodDictionaries.DNA;
        public List<string> Decrypt(string input, StringInfo info)
        {
            input = input.Replace(" ", "");

            int length = input.Length;
            StringBuilder output = new();

            for (int i = 0; i <= length - 3; i += 3)
            {
                string dna = input.Substring(i, Math.Min(3, length - i));
                string? find;

                DNADictionary.TryGetValue(dna, out find);
                if (string.IsNullOrEmpty(find)) { continue; }

                output.Append(find);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;
            if (analysis.Count >= 4 || analysis.Count <= 2) return 1;

            return 0.3;
        }

        public string Name { get { return "DNA"; } }
		public bool RequiresKey { get { return false; } }
    }
}
