using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{
    public class Base64 : IDecoder
    {
        private const string Base64Characters = MethodDictionaries.Base64Characters;

        private bool CustomBase64(string input, out byte[] output)
        {
            // Convert.TryFromBase64String is weird and sometimes doesn't consider base64 without padding as base64

            input = input.TrimEnd('=');
            List<byte> result = new();

            int buffer = 0;
            int bitsLeft = 0;

            foreach (char c in input)
            {
                int val = Base64Characters.IndexOf(c);

                buffer = (buffer << 6) + val;
                bitsLeft += 6;

                if (bitsLeft >= 8)
                {
                    bitsLeft -= 8;
                    result.Add((byte)((buffer >> bitsLeft) & 0xFF)); // only first 8 bits
                }
            }

            output = result.ToArray();
            return true;
        }

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            CustomBase64(input, out byte[] bytes);
            return new() { Encoding.UTF8.GetString(bytes) };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            // 64 + padding character
            if (info.uniqueCharacters <= 2 || info.uniqueCharacters > 65) return 1;
            if (info.minChar < '+' || info.maxChar > 'z') return 1;

            string cleaned = input.TrimEnd('=');
            return DecryptionUtils.IsContainedString(cleaned, Base64Characters) ? 0 : 1;
        }

        public string Name { get { return "Base64"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
