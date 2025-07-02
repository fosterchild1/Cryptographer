using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Morse : IListDecryptionMethod
    {

        private static Dictionary<string, string> MorseDictionary = MethodDictionaries.Morse;

        private string DecryptMorse(string input, string dot, string dash)
        {
            string modifiedInput = input.Replace(dot, ".").Replace(dash, "-");

            // split at each space
            StringBuilder output = new();
            foreach (string morse in modifiedInput.Split(" "))
            {
                Console.WriteLine(morse);
                string? find;
                MorseDictionary.TryGetValue(morse, out find);
                if (string.IsNullOrEmpty(find)) { continue; }

                Console.WriteLine(find);
                output.Append(find);
            }

            return output.ToString();
        }

        public List<string> Decrypt(string input, SortedList<char, int> analysis)
        {
            // morse with no space has to be bruteforced with a looooooooooooooot of cases
            if (analysis.Count < 3)
            {
                return new List<string>()
                {
                    input,
                    input
                };
            }

            // they can be either space, dash or dot
            string c1 = analysis.GetKeyAtIndex(0).ToString();
            string c2 = analysis.GetKeyAtIndex(1).ToString();
            string c3 = analysis.GetKeyAtIndex(2).ToString();

            List<string> output = new()
            {
                DecryptMorse(input, c1, c2),
                DecryptMorse(input, c1, c3),
                DecryptMorse(input, c2, c1),
                DecryptMorse(input, c2, c3),
                DecryptMorse(input, c3, c1),
                DecryptMorse(input, c3, c2),
            };

            return output;
        }

        public string Name { get { return "Morse"; } }
    }
}
