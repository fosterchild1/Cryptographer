using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{
    public class A1Z26 : IDecoder
    {

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            StringBuilder output = new();
            
            foreach (string s in input.Split("-"))
            {
                if (!int.TryParse(s, out int i)) return DecryptionUtils.EmptyResult;
                if (i < 0 || i > 26) return DecryptionUtils.EmptyResult;
                
                char c = (char)i;
                bool isSpace = c == 0;
                output.Append(isSpace ? " " : (char)(c + 64));
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            // 0-9 and a seperator
            if (info.uniqueCharacters <= 2 || info.uniqueCharacters >= 11) return 1;

            // check if it has numbers and then on the numberless string check if it has a seperator
            // if it does have one then the string should only look like "------", and if we trim the first character we should get an empty string

            bool isNumberString = info.IsExclusive(DecryptionUtils.numberHashset);
            if (!isNumberString) return 1;

            return 0.3;
        }

        public string Name { get { return "A1Z26"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
