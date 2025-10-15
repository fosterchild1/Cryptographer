using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Morse : IDecryptionMethod
    {

        private Dictionary<string, string> MorseDictionary = MethodDictionaries.Morse;

        private string DecryptMorse(string input)
        {
            // split at each space
            StringBuilder output = new();
            foreach (string morse in input.Split(" "))
            {
                string? find;
                MorseDictionary.TryGetValue(morse, out find);
                if (string.IsNullOrEmpty(find)) { continue; }

                output.Append(find);
            }

            return output.ToString();
        }

        public List<string> Decrypt(string input, StringInfo info)
        {
            // TODO: permutations
            return new() { DecryptMorse(input) };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;

            // space, dot, dash, slash OR without slash. 3-4 chars
            return ((analysis.Count <= 2 || analysis.Count >= 5) ? 1 : 0);
        }

        public string Name { get { return "Morse"; } }
		public bool RequiresKey { get { return false; } }
    }
}
