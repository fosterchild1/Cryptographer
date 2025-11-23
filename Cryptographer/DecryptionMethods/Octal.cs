using Cryptographer.Classes;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryptographer.DecryptionMethods
{
    public class Octal : IDecryptionMethod
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
                if (!int.TryParse(s, out int integer)) return new();

                string str = ConvertOctal(s);
                if (str == "") return new();
                output.Append(str);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            if (info.uniqueCharacters > 11) return 1; // it should only have 0-9 and a space.

            string withoutNum = Regex.Replace(input, "[0-9]", string.Empty);
            return string.IsNullOrWhiteSpace(withoutNum) ? 0.3 : 1;
        }

        public string Name { get { return "Octal"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
