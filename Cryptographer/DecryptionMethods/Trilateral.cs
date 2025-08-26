using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryptographer.DecryptionMethods
{
    public class Trilateral : IDecryptionMethod
    {
        private readonly Dictionary<string, char> TrilateralSwapped = MethodDictionaries.TrilateralSwapped;
        private readonly Dictionary<string, char> TrilateralNormal = MethodDictionaries.TrilateralNormal;

        private string DecodeTrilateral(string input, Dictionary<string, char> dict)
        {
            int length = input.Length;
            StringBuilder output = new();

            for (int i = 0; i < input.Length; i += 3)
            {
                string trilateral = input.Substring(i, Math.Min(3, length - i));

                if (!dict.TryGetValue(trilateral, out char c)) return "";
                output.Append(c);
            }

            return output.ToString();
        }

        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            return new() {
                DecodeTrilateral(input, TrilateralNormal),
                DecodeTrilateral(input, TrilateralSwapped)
            };
        }

        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            if (analysis.Count != 3 || input.Length % 3 != 0) return 1; // it should only have A, B, C

            return 0.6; // havent seen it much really
        }

        public string Name { get { return "Trilateral"; } }
    }
}
