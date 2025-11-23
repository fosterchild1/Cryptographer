using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{
    public class Base45 : IDecoder
    {
        private Dictionary<char, int> Base45Map = MethodDictionaries.Base45Map;

        private int CharToVal(char c)
        {
            bool success = Base45Map.TryGetValue(c, out int i);
            return (!success ? 255 : i);
        }
        private bool TryFromBase45String(string input, out byte[]? output)
        {
            output = null;

            List<byte> result = new();

            // pad
            input += new string('0', (3 - (input.Length % 3)) % 3);

            for (int i = 0; i < input.Length; i += 3)
            {
                //n=c+(d×45)+(e×45^2)
                byte c = (byte)CharToVal(input[i]);
                byte d = (byte)CharToVal(input[i + 1]);
                byte e = (byte)CharToVal(input[i + 2]);
                if (c == 255 || d == 255 || e == 255) return false; // aka failed in chartoval

                int n = c + (d * 45) + (e * 45 * 45);
                byte secondByte = (byte)((n >> 8) & 0xFF);

                if (secondByte != 0) result.Add(secondByte);
                result.Add((byte)(n & 0xFF));
            }

            output = result.ToArray();
            return true;
        }

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            TryFromBase45String(input, out byte[]? output);

            return new() { output != null ? Encoding.UTF8.GetString(output) : "" };
        }


        public double CalculateProbability(string input, StringInfo info)
        {
            // 45 chars
            if (info.uniqueCharacters <= 2 || info.uniqueCharacters > 45) return 1;
            if (info.minChar < '$' || info.maxChar > 'Z') return 1;

            return !TryFromBase45String(input, out byte[]? output) ? 1 : 0;
        }

        public string Name { get { return "Base45"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
