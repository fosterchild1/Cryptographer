using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base85 : IDecryptionMethod
    {
        private Dictionary<char, int> Base85Map = MethodDictionaries.Base85Map;
        private bool TryFromBase85String(string input, out byte[]? output)
        {
            output = null;

            List<byte> result = new();

            int count = 0;
            long buffer = 0;

            foreach (char c in input)
            {
                if (!Base85Map.TryGetValue(c, out int val)) return false;

                // add byte onto the buffer
                buffer = buffer * 85 + val;
                count++;

                if (count >= 5)
                {;

                    result.Add((byte)(buffer >> 24)); // get first byte
                    result.Add((byte)(buffer >> 16));// second
                    result.Add((byte)(buffer >> 8)); // third
                    result.Add((byte)buffer); // fourth

                    count = 0;
                    buffer = 0;
                }
            }

            if (count > 0)
            {
                // padding
                for (int i = count; i < 5; i++)
                    buffer = buffer * 85 + 84;

                for (int i = 0; i < count - 1; i++)
                    result.Add((byte)(buffer >> (24 - i * 8)));
            }

            output = result.ToArray();
            return true;
        }
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            TryFromBase85String(input, out byte[]? output);
            //Console.WriteLine(Encoding.UTF8.GetString(output));
            return new() { output != null ? Encoding.UTF8.GetString(output) : "" };
        }


        public double CalculateProbability(string input, StringInfo info)
        {
            // 85 + padding character
            if (info.uniqueCharacters <= 2 || info.uniqueCharacters > 86) return 1;
            if (info.minChar < '!' || info.maxChar > 'z') return 1;

            return !TryFromBase85String(input, out byte[]? output) ? 1 : 0;
        }

        public string Name { get { return "Base85"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
