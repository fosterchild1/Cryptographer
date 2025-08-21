using Cryptographer.Classes;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryptographer.DecryptionMethods
{
    public class Octal : IDecryptionMethod
    {
        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            StringBuilder output = new();
            foreach (string s in input.Split(" "))
            {
                if (!int.TryParse(s, out int integer)) return new();
                if (integer < 0 || integer > 255) return new();

                int dec = Convert.ToInt32(s, 8);
                output.Append((char)dec);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            if (analysis.Count > 11) return 1; // it should only have 0-9 and a space.

            string withoutNum = Regex.Replace(input, "[0-9]", string.Empty);
            return string.IsNullOrWhiteSpace(withoutNum) ? 0.3 : 1;
        }

        public string Name { get { return "Octal"; } }
    }
}
