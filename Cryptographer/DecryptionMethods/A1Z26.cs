using Cryptographer.Classes;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryptographer.DecryptionMethods
{
    public class A1Z26 : IDecryptionMethod
    {

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            StringBuilder output = new();
            
            foreach (string s in input.Split("-"))
            {
                if (!int.TryParse(s, out int i)) return new();
                if (i < 0 || i > 26) return new();

                char c = (char)i;
                bool isSpace = c == 0;
                output.Append(isSpace ? " " : (char)(c + 64));
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            // 0-9 and a seperator
            var analysis = info.frequencyAnalysis;
            if (analysis.Count <= 2 || analysis.Count >= 11) return 1;

            // check if it has numbers and then on the numberless string check if it has a seperator
            // if it does have one then the string should only look like "------", and if we trim the first character we should get an empty string

            string withoutNum = Regex.Replace(input, "[0-9]", string.Empty);

            if (withoutNum == string.Empty) return 0.3;
            return withoutNum.Trim(withoutNum[0]) == string.Empty ? 0.3 : 1;
        }

        public string Name { get { return "A1Z26"; } }
		public bool RequiresKey { get { return false; } }
		public bool IsFallback { get { return false; } }
    }
}
