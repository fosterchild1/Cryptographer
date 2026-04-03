using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{
    public class Octal : IDecoder
    {
        private string ConvertOctal(string oct)
        {
            try
            {
                int val = Convert.ToInt32(oct, 8);
                return ((char)val).ToString();
            }
            catch
            {
                return "";
            }
        }

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            StringBuilder output = new();
            foreach (string s in input.Split(" "))
            {
                if (!int.TryParse(s, out int integer)) return DecryptionUtils.EmptyResult;

                string str = ConvertOctal(s);
                if (str == "") return DecryptionUtils.EmptyResult;
                output.Append(str);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            if (info.uniqueCharacters > 11) return 1; // it should only have 0-9 and a space.

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
