using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Morse : IDecryptionMethod
    {

        private static Dictionary<string, string> MorseDictionary = MethodDictionaries.Morse;

        private string DecryptMorse(string input, string dot, string dash)
        {
            string modifiedInput = input.Replace(dot, ".").Replace(dash, "-").Replace("/", " / ");

            // split at each space
            StringBuilder output = new();
            foreach (string morse in modifiedInput.Split(" "))
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
            // morse with no space has to be bruteforced with a looooooooooooooot of cases
            if (analysis.Count < 3 || analysis.Count > 3)
            {
                return new List<string>();
            }

            // they can be dash or dot
            string c1 = analysis[0].Key.ToString();
            string c2 = analysis[1].Key.ToString();

            List<string> output = new()
            {
                DecryptMorse(input, c1, c2),
                DecryptMorse(input, c2, c1),
            };

            return output;
        }

        public string Name { get { return "Morse"; } }
    }
}
