using Cryptographer.Utils;
using System.Reflection;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base32 : IDecryptionMethod
    {
        private static Dictionary<char, int> Base32Map = MethodDictionaries.Base32Map;
        private static bool TryFromBase32String(string input, out byte[]? output)
        {
            output = null;

            input = input.TrimEnd('=').ToUpper();
            byte[] bytes = Encoding.ASCII.GetBytes(input);

            List<byte> result = new();

            int buffer = 0;
            int bitsLeft = 0;

            foreach (char c in input)
            {
                if (!Base32Map.TryGetValue(c, out int val)) return false;

                // make space for new 5-bit value and add it onto buffer
                buffer = (buffer << 5) | val;
                bitsLeft += 5;

                if (bitsLeft >= 8)
                {
                    bitsLeft -= 8;
                    result.Add((byte)((buffer >> bitsLeft) & 0xFF));
                }
            }

            output = result.ToArray();
            return true;
        }

        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            if (!TryFromBase32String(input, out byte[]? output)) return new();

            return new() { output != null ? Encoding.UTF8.GetString(output) : "" };
        }

        public string Name { get { return "Base32"; } }
    }
}
