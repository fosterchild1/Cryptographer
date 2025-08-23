using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{

    public class Binary : IDecryptionMethod
    {
        private string DecryptBinary(string input)
        {
            input = input.Replace(" ", "");
            int length = input.Length;

            StringBuilder sb = new();

            for (int i = 0; i <= length - 8; i += 8)
            {
                try
                {
                    string bin = input.Substring(i, Math.Min(8, length - i));
                    char c = (char)Convert.ToInt32(bin, 2);

                    // outside binary range
                    if (c > 255 || c < 0)
                        continue;

                    sb.Append(c);
                }
                catch { }
            }

            return sb.ToString();
        }
        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            // zero, one and a space
            if (analysis.Count > 3)
                return new();

            return new() { DecryptBinary(input) };
        }
        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            return (analysis.Count > 3 ? 1 : 0);
        }

        public string Name { get { return "Binary"; } }
    }
}
