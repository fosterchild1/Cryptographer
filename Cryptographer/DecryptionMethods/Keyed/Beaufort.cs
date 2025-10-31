using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Beaufort : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info, string key)
        {
            // Decoding is vigenere but instead of text - key its key - text.
            StringBuilder output = new();
            int keyLength = key.Length;
            int keyIdx = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                char keyC = key[keyIdx % keyLength].ToString().ToUpper().ToCharArray()[0]; // this is stupid
                keyIdx++;

                if ('a' <= c && 'z' >= c)
                {
                    int keyed = keyC + 32 - c; //  (keyC - 65) - (c - 97)

                    output.Append((char)(97 + (keyed < 0 ? keyed + 26 : keyed)));
                    continue;
                }

                if ('A' <= c && 'Z' >= c)
                {
                    int keyed = keyC - c;

                    output.Append((char)(65 + (keyed < 0 ? keyed + 26 : keyed)));
                    continue;
                }

                keyIdx--; // not a-z or A-Z
                output.Append(c);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return 0.35;
        }

        public string Name { get { return "Beaufort"; } }
        public KeyLevel RequiredKey { get { return KeyLevel.Keyed; } }
        public bool IsFallback { get { return true; } }
    }
}
