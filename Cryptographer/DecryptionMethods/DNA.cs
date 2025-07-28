using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class DNA : IDecryptionMethod
    {
        private static Dictionary<string, string> DNADictionary = MethodDictionaries.DNA;
        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            if (analysis.Count != 4) return new List<string>();

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

        public string Name { get { return "DNA"; } }
    }
}
