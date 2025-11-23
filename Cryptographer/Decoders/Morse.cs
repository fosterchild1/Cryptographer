using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{
    public class Morse : IDecoder
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

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            // TODO: permutations
            return new() { DecryptMorse(input) };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            if (info.uniqueCharacters <= 2 || info.uniqueCharacters >= 5) return 1;

            // space, dot, dash, slash OR without slash. 3-4 chars
            return 0;
        }

        public string Name { get { return "Morse"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
