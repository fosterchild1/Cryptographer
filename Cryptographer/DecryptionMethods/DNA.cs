using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class DNA : IDecryptionMethod
    {
        private Dictionary<string, string> DNADictionary = MethodDictionaries.DNA;
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            input = input.Replace(" ", "");

            int length = input.Length;
            StringBuilder output = new();

            for (int i = 0; i <= length - 3; i += 3)
            {
                string dna = input.Substring(i, Math.Min(3, length - i));

                DNADictionary.TryGetValue(dna, out string? find);
                if (string.IsNullOrEmpty(find)) continue;

                output.Append(find);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            if (info.uniqueCharacters >= 4 || info.uniqueCharacters <= 2) return 1;

            return 0.3;
        }

        public string Name { get { return "DNA"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
