using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Trilateral : IDecryptionMethod
    {
        private readonly Dictionary<string, char> TrilateralSwapped = MethodDictionaries.TrilateralSwapped;
        private readonly Dictionary<string, char> TrilateralNormal = MethodDictionaries.TrilateralNormal;

        private string DecodeTrilateral(string input, char a, char b, char c, Dictionary<string, char> dict)
        {
            input = input.Replace(a, 'A').Replace(b, 'B').Replace(c, 'C');

            int length = input.Length;
            StringBuilder output = new();

            for (int i = 0; i < input.Length; i += 3)
            {
                string trilateral = input.Substring(i, Math.Min(3, length - i));

                if (!dict.TryGetValue(trilateral, out char decodedChar)) return "";
                output.Append(decodedChar);
            }

            return output.ToString();
        }

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            // since this has a really easy way of detecting whether it is trilateral or not,
            // we can check for character substitutions (eg. A = D, B = E, C = F) even though its expensive

            var analysis = info.frequencyAnalysis;

            char c1 = analysis[0].Key;
            char c2 = analysis[1].Key;
            char c3 = analysis[2].Key;

            List<List<char>> permutations = new(); DecryptionUtils.GetPermutations(new() { c1, c2, c3 }, new(), permutations);
            List<string> output = new();

            foreach (List<char> perm in permutations)
            {
                output.Add(DecodeTrilateral(input, perm[0], perm[1], perm[2], TrilateralNormal));
                output.Add(DecodeTrilateral(input, perm[0], perm[1], perm[2], TrilateralSwapped));
            }

            return output;
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            if (info.uniqueCharacters != 3 || input.Length % 3 != 0) return 1; // it should only have A, B, C

            return 0.05;
        }

        public string Name { get { return "Trilateral"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
