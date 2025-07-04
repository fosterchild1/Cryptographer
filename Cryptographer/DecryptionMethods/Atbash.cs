using System.Text;

namespace Cryptographer.DecryptionMethods
{

    public class Atbash : IDecryptionMethod
    {
        public string Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {           
            char[] chars = input.ToCharArray();
            StringBuilder sb = new();

            foreach (int c in chars)
            {
                // between A - Z
                if (c > 64 && c < 91)
                {
                    char atbashedUpper = (char)(90 - c + 65);
                    sb.Append(atbashedUpper);
                    continue;
                }

                // between a - z
                if (c > 96 && c < 123)
                {
                    char atbashed = (char)(122 - c + 97);
                    sb.Append(atbashed);
                    continue;
                }

                sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string Name { get { return "Atbash"; } }
    }
}
