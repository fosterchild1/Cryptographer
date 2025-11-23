using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.Decoders
{
    public class Atbash : IDecoder
    {
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            char[] chars = input.ToCharArray();
            StringBuilder sb = new();

            foreach (int c in chars)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    char atbashedUpper = (char)(90 - c + 65);
                    sb.Append(atbashedUpper);
                    continue;
                }

                // between a - z
                if (c >= 'a' && c <= 'z')
                {
                    char atbashed = (char)(122 - c + 97);
                    sb.Append(atbashed);
                    continue;
                }

                sb.Append((char)c);
            }

            return new() { sb.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            // if it has just 2 characters, that means its either morse, bacon or binary. we already change the characters in those
            // so we dont need to change
            return (info.uniqueCharacters <= 2 ? 1 : 0.6);
        }

        public string Name { get { return "Atbash"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return true; } }
    }
}
