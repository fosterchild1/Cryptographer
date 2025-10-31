using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Numerics;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base58 : IDecryptionMethod
    {
        private Dictionary<string, string> base58Alphabets = new()
        {
            ["BTC"] = MethodDictionaries.Base58BTC,
            ["Flickr"] = MethodDictionaries.Base58Flickr,
            ["Ripple"] = MethodDictionaries.Base58Ripple,
        };

        private Dictionary<string, Dictionary<char, int>> base58Maps = new();

        private bool TryFromBase58String(string input, Dictionary<char, int> dict, out byte[]? output)
        {
            output = default!;

            BigInteger buffer = 0;

            foreach (char c in input)
            {
                if (!dict.TryGetValue(c, out int val)) return false;

                buffer = buffer * 58 + val;
            }

            output = buffer.ToByteArray().Reverse().ToArray(); // we have to reverse it. stupid.
            return true;
        }

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            List<string> outputs = new();
            foreach (KeyValuePair<string, Dictionary<char, int>> kvp in base58Maps)
            {
                Dictionary<char, int> dict = kvp.Value;
                TryFromBase58String(input, dict, out byte[]? output);
                if (output == null) continue;

                outputs.Add(Encoding.UTF8.GetString(output));
            }

            return outputs;
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;
            if (analysis.Count <= 2 || analysis.Count > 58) return 1;
            if (info.minChar < 48 || info.maxChar > 122) return 1; // between 0-1, A-Z and a-z

            return 0.3;
        }

        // init dictionaries
        public Base58()
        {
            foreach (KeyValuePair<string, string> kvp in base58Alphabets)
            {
                string alphabet = kvp.Value;
                Dictionary<char, int> map = new();

                for (int i = 0; i < alphabet.Length; i++)
                {
                    map[alphabet[i]] = i;
                }

                base58Maps[kvp.Key] = map;
            }
        }

        public string Name { get { return "Base58"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
