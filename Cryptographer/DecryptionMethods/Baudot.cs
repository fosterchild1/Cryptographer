using Cryptographer.Classes;
using Cryptographer.Utils;
using System;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Baudot : IDecryptionMethod
    {

        private static Dictionary<string, char> BaudotDictionary = MethodDictionaries.Baudot;

        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            StringBuilder output = new();

            bool useFigures = false;
            foreach (string s in input.Split(" "))
            {
                // baudot is stricly 5 characters
                if (s.Length != 5) return new();

                if (s == "11011") useFigures = true;
                if (s == "11111") useFigures = false;

                if (!BaudotDictionary.TryGetValue(useFigures ? s + "_" : s, out char c)) return new();
                output.Append(c);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            // baudot only has 0s, 1s and a space
            // not seen that much
            return (analysis.Count > 3 ? 1 : 0.4);
        }

        public string Name { get { return "Baudot"; } }
    }
}
