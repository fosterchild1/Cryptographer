using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base45 : IDecryptionMethod
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

            int length = input.Length;
            int i = 0;

            while (true)
            {
                //n=c+(d×45)+(e×45^2)
                byte c = (byte)CharToVal(input[i]);
                byte d = (byte)CharToVal(input[i + 1]);
                byte e = (byte)CharToVal(input[i + 2]);
                if (c == 255 || d == 255 || e == 255) return false; // aka failed in chartoval

                int n = c + (d * 45) + (e * 45 * 45);

                result.Add((byte)((n >> 8) & 0xFF));
                result.Add((byte)((n) & 0xFF));

                if (i == length - 3) break;

                i += 3;
                if (i > length - 3) // add zeros
                {
                    input += new string('0', 3 - (length % 3));
                    length = input.Length;
                }
            }

            output = result.ToArray();
            return true;
        }

        public List<string> Decrypt(string input, StringInfo info)
        {
            TryFromBase45String(input, out byte[]? output);

            return new() { output != null ? Encoding.UTF8.GetString(output) : "" };
        }


        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;
            // 45 chars
            if (analysis.Count <= 2 || analysis.Count > 45 || info.maxChar > 90) return 1; // > 90 means >Z but we only have characters 0-1 and A-Z and stuff between

            return !TryFromBase45String(input, out byte[]? output) ? 1 : 0;
        }

        public string Name { get { return "Base45"; } }
		public bool RequiresKey { get { return false; } }
    }
}
