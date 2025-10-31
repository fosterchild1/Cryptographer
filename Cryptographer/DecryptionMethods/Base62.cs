using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Numerics;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base62 : IDecryptionMethod
    {
        private Dictionary<char, int> Base62Map = MethodDictionaries.Base62Map;
        private bool TryFromBase62String(string input, out byte[]? output)
        {
            output = default!;

            BigInteger buffer = 0;

            foreach (char c in input)
            {
                if (!Base62Map.TryGetValue(c, out int val)) return false;

                buffer = buffer * 62 + val;
            }

            output = buffer.ToByteArray().Reverse().ToArray(); // we have to reverse it. stupid.
            return true;
        }

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            TryFromBase62String(input, out byte[]? output);
            return new() { output != null ? Encoding.UTF8.GetString(output) : "" };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;
            if (analysis.Count <= 2 || analysis.Count > 62) return 1;

            return TryFromBase62String(input, out byte[]? _) ? 0 : 1;
        }

        public string Name { get { return "Base62"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
