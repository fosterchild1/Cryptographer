using Cryptographer.Classes;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryptographer.Decoders
{
    public class Hexadecimal : IDecoder
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
            if (info.uniqueCharacters > 17 || info.uniqueCharacters <= 3) return 1;

            string reg = Regex.Replace(input, "[0-9a-f]", string.Empty);
            return string.IsNullOrWhiteSpace(reg) ? 0.2 : 1;
        }

        public string Name { get { return "Hexadecimal"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
