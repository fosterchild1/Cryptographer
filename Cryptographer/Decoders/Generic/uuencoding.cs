using Cryptographer.Classes;
using System.Text;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
namespace Cryptographer.Decoders
{
    public class uuencoding : IDecoder
    {
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            List<byte> result = new();

            foreach (string line in input.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("begin") || line.EndsWith("end")) continue;

                int buffer = 0;
                int count = 0;

                for (int i = 1; i < line.Length; i++) //  // first char = length of line
                {
                    // subtract 32 from each character's ASCII code (modulo 64 to account for the grave accent usage)
                    // to get a 6-bit value, concatenate 4 6-bit groups to get 24 bits, then output 3 bytes. 
                    int val = (line[i] - 32) % 64;
                    buffer = (buffer << 6) + val;
                    count++;

                    if (count < 4) continue;

                    result.Add((byte)((buffer >> 16) & 0xFF));
                    result.Add((byte)((buffer >> 8) & 0xFF));
                    result.Add((byte)(buffer & 0xFF));

                    buffer = 0;
                    count = 0;
                }
            }

            return new() { Encoding.ASCII.GetString(result.ToArray()) };
        }

        private HashSet<char> importantChars = new() {' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', 
            '-', '.', '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '_'};

        public double CalculateProbability(string input, StringInfo info)
        {
            // legit the exact same as rot 47 but with 64 chars

            // alphabet of 64 chars
            if (info.uniqueCharacters <= 2 || info.uniqueCharacters > 64) return 1;

            double currentCount = info.Exists(importantChars).Count;
            return Math.Pow(0.9, currentCount);
        }

        public string Name { get { return "uuencoding"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
