using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Hexadecimal : IDecryptionMethod
    {
        private string ConvertHex(string hex)
        {
            try
            {
                int val = Convert.ToInt32(hex, 16);
                return char.ConvertFromUtf32(val);
            }
            catch {
                return "";
            }
        }

        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            input = input.Replace("-", "");

            StringBuilder output = new();
            foreach (string hex in input.Split(" "))
            {
                string converted = ConvertHex(hex);
                if (string.IsNullOrEmpty(converted)) return new();

                output.Append(ConvertHex(hex));
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            if (analysis.Count > 17 || analysis.Count == 1) return 1;

            return 0.7;
        }

        public string Name { get { return "Hexadecimal"; } }
    }
}
