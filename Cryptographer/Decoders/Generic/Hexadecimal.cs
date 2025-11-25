using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

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
                if (string.IsNullOrEmpty(converted)) return DecryptionUtils.EmptyResult;

                output.Append(converted);
            }

            return new() { output.ToString() };
        }

        private HashSet<char> allowedChars = @"0123456789abcdefABCDEF".ToHashSet();

        public double CalculateProbability(string input, StringInfo info)
        {
            if (info.uniqueCharacters > 17 || info.uniqueCharacters <= 3) return 1;

            return info.IsExclusive(allowedChars) ? 0 : 1;
        }

        public string Name { get { return "Hexadecimal"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
