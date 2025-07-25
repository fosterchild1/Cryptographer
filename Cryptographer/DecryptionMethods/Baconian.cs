﻿using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Baconian : IDecryptionMethod
    {

        // baconian has TWO!! dictionaries!!
        private static Dictionary<string, string> Baconian26 = MethodDictionaries.Baconian26;
        private static Dictionary<string, string> Baconian24 = MethodDictionaries.Baconian24;

        private string DecryptBacon(string input, string ah, string bah, Dictionary<string, string> dict)
        {
            string modifiedInput = input.Replace(ah, "A").Replace(bah, "B").Replace(" ", "");


            StringBuilder output = new();
            int length = modifiedInput.Length;
            // split every 5 characters

            for (int i=0; i <= length - 5; i += 5) 
            {
                string bacon = modifiedInput.Substring(i, Math.Min(5, length - i));
                string? find;
                dict.TryGetValue(bacon, out find);
                if (string.IsNullOrEmpty(find)) { continue; }

                output.Append(find);
            }

            return output.ToString();
        }

        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            // it can have more than 2 characters (ABBAB DCCDD) but thats too performance heavy
            if (analysis.Count != 2)
            {
                return new List<string>();
            }

            // they can be either ah or bah
            string c1 = analysis[0].Key.ToString();
            string c2 = analysis[1].Key.ToString();

            List<string> output = new()
            {
                DecryptBacon(input, c1, c2, Baconian24),
                DecryptBacon(input, c2, c1, Baconian24),
                DecryptBacon(input, c1, c2, Baconian26),
                DecryptBacon(input, c2, c1, Baconian26),
            };

            return output;
        }

        public string Name { get { return "Baconian"; } }
    }
}
