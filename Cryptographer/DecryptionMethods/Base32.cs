using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base32 : IDecryptionMethod
    {
        private Dictionary<char, int> Base32Map = MethodDictionaries.Base32Map;
        private bool TryFromBase32String(string input, out byte[]? output)
        {
            output = null;

            input = input.TrimEnd('=').ToUpper();

            List<byte> result = new();

            int buffer = 0;
            int bitsLeft = 0;

            foreach (char c in input)
            {
                if (!Base32Map.TryGetValue(c, out int val)) return false;

                // make space for new 5-bit value and add it onto buffer
                buffer = (buffer << 5) + val;
                bitsLeft += 5;

                if (bitsLeft >= 8)
                {
                    bitsLeft -= 8;
                    result.Add((byte)((buffer >> bitsLeft) & 0xFF)); // only first 8 bits
                }
            }

            output = result.ToArray();
            return true;
        }

        public List<string> Decrypt(string input, StringInfo info)
        {
            TryFromBase32String(input, out byte[]? output);
           // Console.WriteLine(Encoding.UTF8.GetString(output));
            return new() { output != null ? Encoding.UTF8.GetString(output) : "" };
        }


        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;
            // 32 + padding character
            if (analysis.Count <= 2 || analysis.Count > 32) return 1;

            return !TryFromBase32String(input, out byte[]? output) ? 1 : 0;
        }

        public string Name { get { return "Base32"; } }
		public bool RequiresKey { get { return false; } }
    }
}
