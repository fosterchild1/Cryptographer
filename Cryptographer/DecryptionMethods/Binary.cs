using System.Text;

namespace Cryptographer.DecryptionMethods
{

    public class Binary : IDecryptionMethod
    {
        public string DecryptBinary(string input, string zero, string one)
        {
            string modifiedInput = input.Replace(zero, "0").Replace(one, "1");
            int length = modifiedInput.Length;

            StringBuilder sb = new();

            for (int i = 0; i <= length - 8; i += 8)
            {
                try
                {
                    string bin = modifiedInput.Substring(i, Math.Min(8, length - i));
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
            // it can be stuff like: ABBABBCCDCCDD where it changes mid way. thats too performance heavy to check each case.
            if (analysis.Count != 2)
                return new List<string>();

            // they can be either one or zero
            string c1 = analysis[0].Key.ToString();
            string c2 = analysis[1].Key.ToString();

            return new List<string>()
            {
                DecryptBinary(input, c1, c2),
                DecryptBinary(input, c2, c1)
            };
        }
        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            return (analysis.Count != 2 ? 1 : 0);
        }

        public string Name { get { return "Binary"; } }
    }
}
