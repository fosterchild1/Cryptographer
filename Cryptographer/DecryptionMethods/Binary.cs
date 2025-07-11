using System.Text;

namespace Cryptographer.DecryptionMethods
{

    public class Binary : IDecryptionMethod
    {
        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {

            string modifiedInput = ProjUtils.RemoveWhitespaces(input);
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

           return new List<string>() { sb.ToString() };
        }

        public string Name { get { return "Binary"; } }
    }
}
