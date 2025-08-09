using System.Text;

namespace Cryptographer.DecryptionMethods
{

    public class Atbash : IDecryptionMethod
    {
        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
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

           return new List<string>() { sb.ToString() };
        }

        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            // if it has just 2 characters, that means its either morse, bacon or binary. we already change the characters in those
            // so we dont need to change
            return (analysis.Count <= 2 ? 1 : 0.7);
        }

        public string Name { get { return "Atbash"; } }
    }
}
