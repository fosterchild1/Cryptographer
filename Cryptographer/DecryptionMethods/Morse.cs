using Cryptographer.Classes;
using Cryptographer.Utils;
using System;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Morse : IDecryptionMethod
    {

        private static Dictionary<string, string> MorseDictionary = MethodDictionaries.Morse;

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

        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {

            return new() { DecryptMorse(input) };
        }

        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            // morse with no space has to be bruteforced with a lot of cases
            return (analysis.Count > 4 ? 1 : 0);
        }

        public string Name { get { return "Morse"; } }
    }
}
