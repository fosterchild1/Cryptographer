using Cryptographer.Classes;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryptographer.DecryptionMethods
{
    public class Caesar : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            List<string> output = new();
            for (int i = 0; i <= 25; i++) // 26 = no transformation
            {
                StringBuilder transformed = new();
                foreach (char c in input) 
                {
                    if (c >= 'A' && c <= 'Z')
                        transformed.Append((char)(((c - 'A' + i) % 26) + 'A'));

                    else if (c >= 'a' && c <= 'z')
                        transformed.Append((char)(((c - 'a' + i) % 26) + 'a'));

                    else
                        transformed.Append(c);
                }

                output.Add(transformed.ToString());
            }
            return output;
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            // check if its just numbers
            string withoutNum = Regex.Replace(input, "[0-9]", string.Empty);
            if (string.IsNullOrWhiteSpace(withoutNum))
                return 1;

            return 0.4;
        }

        public string Name { get { return "Caesar"; } }
		public bool RequiresKey { get { return false; } }
    }
}
