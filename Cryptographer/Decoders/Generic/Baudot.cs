using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{
    public class Baudot : IDecoder
    {
        private Dictionary<string, char> BaudotDictionary = MethodDictionaries.Baudot;

        private const string SWITCH_TO_FIGURES = "11011";
        private const string SWITCH_TO_LETTERS = "111111";

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            StringBuilder output = new();

            bool useFigures = false;
            foreach (string s in input.Split(" "))
            {
                // baudot is stricly 5 characters
                if (s.Length != 5) return DecryptionUtils.EmptyResult;

                if (s == SWITCH_TO_FIGURES) useFigures = true;
                if (s == SWITCH_TO_LETTERS) useFigures = false;

                if (!BaudotDictionary.TryGetValue(useFigures ? s + "_" : s, out char c)) return DecryptionUtils.EmptyResult;
                output.Append(c);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            // baudot only has 0s, 1s and a space
            if (info.uniqueCharacters > 3) return 1;

            // not seen that much
            return 0.4; // 0.4 relative to baconian, morse, binary etc
        }

        public string Name { get { return "Baudot"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
