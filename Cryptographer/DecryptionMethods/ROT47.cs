using Cryptographer.Classes;
using System.Text;

// placeholder
namespace Cryptographer.DecryptionMethods
{
    public class ROT47 : IDecryptionMethod
    {
        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
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

        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            // alphabet of 94 chars
            if (analysis.Count <= 2 || analysis.Count > 94) return 1;

            // MIN((32 - X) / 32, 0.8)
            int importantCount = importantChars.Count;
            double currentCount = FrequencyAnalysis.Exists(analysis, importantChars).Count;

            double probability = Math.Min((importantCount - currentCount) / importantCount, 0.8);
            return probability;
        }

        public string Name { get { return "ROT47"; } }
    }
}
