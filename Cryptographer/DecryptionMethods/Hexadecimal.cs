using Cryptographer.Classes;
using System.Text;
using System.Text.RegularExpressions;

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

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            input = input.Replace("-", "");

            StringBuilder output = new();
            foreach (string hex in input.Split(" "))
            {
                string converted = ConvertHex(hex);
                if (string.IsNullOrEmpty(converted)) return new();

                output.Append(converted);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;
            if (analysis.Count > 17 || analysis.Count <= 3) return 1;

            string reg = Regex.Replace(input, "[0-9a-f]", string.Empty);
            return string.IsNullOrWhiteSpace(reg) ? 0.2 : 1;
        }

        public string Name { get { return "Hexadecimal"; } }
		public bool RequiresKey { get { return false; } }
		public bool IsFallback { get { return false; } }
    }
}
