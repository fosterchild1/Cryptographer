using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{
    public class ASCII : IDecoder
    {
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            StringBuilder output = new();
            foreach (string s in input.Split(" "))
            {
                // handle double/triple/quadruple/... spaces between nums
                if (string.IsNullOrWhiteSpace(s)) continue;

                if (!int.TryParse(s, out int integer)) return DecryptionUtils.EmptyResult;
                if (integer <= 0 || integer > 127) return DecryptionUtils.EmptyResult;

                output.Append((char)integer);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            if (info.uniqueCharacters > 11) return 1; // it should only have 0-9 and a space.

            // is only numbers
            if (!info.IsExclusive(DecryptionUtils.numberHashset))
                return 1;

            return 0.1;
        }

        public string Name { get { return "ASCII"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
