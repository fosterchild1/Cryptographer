using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{
    public class Octal : IDecoder
    {
        private char ConvertOctal(string oct)
        {
            try
            {
                int val = Convert.ToInt32(oct, 8);
                return (char)val;
            }
            catch
            {
                return '\0';
            }
        }

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            StringBuilder output = new();
            foreach (string s in input.Split(" "))
            {
                char converted = ConvertOctal(s);
                if (converted == '\0') return DecryptionUtils.EmptyResult;

                output.Append(converted);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            if (info.uniqueCharacters > 8) return 1; // it should only have 0-7 and a space.

            // is only numbers
            if (!info.IsExclusive(DecryptionUtils.numberHashset))
                return 1;

            return 0.3;
        }

        public string Name { get { return "Octal"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
