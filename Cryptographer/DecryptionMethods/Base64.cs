using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base64 : IDecryptionMethod
    {
        private Dictionary<char, int> Base64Map = MethodDictionaries.Base64Map;
        private bool CustomBase64(string input, out byte[]? output)
        {
            // Convert.TryFromBase64String is weird and sometimes doesn't consider base64 without padding as base64

            output = default!;

            input = input.TrimEnd('=');
            List<byte> result = new();

            int buffer = 0;
            int bitsLeft = 0;

            foreach (char c in input)
            {
                if (!Base64Map.TryGetValue(c, out int val)) return false;

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
            CustomBase64(input, out byte[]? bytes);
            string output = Encoding.UTF8.GetString(bytes!);
 
            return new() { output };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            // 64 + padding character
            if (info.uniqueCharacters <= 2 || info.uniqueCharacters > 65) return 1;

            return !CustomBase64(input, out byte[]? bytes) ? 1 : 0;
        }
        public string Name { get { return "Base64"; } }
		public bool RequiresKey { get { return false; } }
		public bool IsFallback { get { return false; } }
    }
}
