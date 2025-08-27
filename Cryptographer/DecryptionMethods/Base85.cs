using Cryptographer.Classes;
using Cryptographer.Utils;
using System;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base85 : IDecryptionMethod
    {
        private Dictionary<char, int> Base85Map = MethodDictionaries.Base85Map;
        private bool TryFromBase85String(string input, out byte[]? output)
        {
            output = null;

            byte[] bytes = Encoding.ASCII.GetBytes(input);

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
        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            TryFromBase85String(input, out byte[]? output);
            Console.WriteLine(Encoding.UTF8.GetString(output));
            return new() { output != null ? Encoding.UTF8.GetString(output) : "" };
        }


        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            // 85 + padding character
            if (analysis.Count <= 2 || analysis.Count > 86) return 1;

            return !TryFromBase85String(input, out byte[]? output) ? 1 : 0;
        }

        public string Name { get { return "Base85"; } }
    }
}
