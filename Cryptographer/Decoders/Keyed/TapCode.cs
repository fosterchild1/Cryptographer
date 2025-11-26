using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{
    public class TapCode : IDecoder
    {
        private readonly string DefaultTapAlphabet = MethodDictionaries.DefaultTapAlphabet;

        public List<string> Decrypt(string input, StringInfo info, string key)
        {
            key = string.IsNullOrEmpty(key) ? DefaultTapAlphabet : key;

            // Decoding is vigenere but instead of text - key its key - text.
            StringBuilder output = new();
            int length = input.Length;

            string modifiedInput = input.Replace(info.frequencyAnalysis[1].Key, ' ').TrimStart(' ').TrimEnd(' ');
            string[] split = modifiedInput.Split(" ");
            for (int i = 1; i < split.Length; i+=2)
            {
                int curLength = split[i].Length;
                int prevLength = split[i - 1].Length;

                if (curLength > 5 || curLength <= 0) continue;
                if (prevLength > 5 || prevLength <= 0) continue;

                int keyIdx = 5 * (prevLength - 1) + curLength - 1; // see polybius
                output.Append(key[keyIdx]);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return info.uniqueCharacters == 2 ? 0 : 1;
        }

        public string Name { get { return "Tap Code"; } }
        public KeyLevel RequiredKey { get { return KeyLevel.KeyedWithDefault; } }
        public bool IsFallback { get { return false; } }
    }
}
