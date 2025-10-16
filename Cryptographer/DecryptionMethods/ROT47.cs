﻿using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class ROT47 : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            StringBuilder output = new();
            foreach (char c in input)
            {
                if (c < 33 || c > 126)
                {
                    output.Append(c);
                    continue;
                }

                output.Append((char)(33 + ((c + 14) % 94)));
            }

            return new() { output.ToString() };
        }

        // list of all non alphanumerical characters
        private HashSet<char> importantChars = new() {'!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', ':', ';', '<', '=',
            '>', '?', '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~'};

        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;

            // alphabet of 94 chars
            if (analysis.Count <= 2 || analysis.Count > 94) return 1;

            double currentCount = info.Exists(importantChars).Count;

            return Math.Pow(0.9, currentCount);
        }

        public string Name { get { return "ROT-47"; } }
		public bool RequiresKey { get { return false; } }
		public bool IsFallback { get { return false; } }
    }
}
