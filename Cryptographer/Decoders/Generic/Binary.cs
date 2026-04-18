using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{

    public class Binary : IDecoder
    {

        private string DecodeBinary(string input, char zero, char one, char space)
        {
            input = input.Replace(one, '1').Replace(zero, '0').Replace(space.ToString(), "");
            int length = input.Length;

            StringBuilder sb = new(length/8);

            for (int i = 0; i <= length - 8; i += 8)
            {
                string bin = input.Substring(i, Math.Min(8, length - i));
                char c = DecryptionUtils.FromBase(bin, 2);

                // outside binary range
                if (c > 255 || c < 1)
                    return "";

                sb.Append(c);
            }

            return sb.ToString();
        }
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            // since this has a really safe way of detecting whether it is binary or not,
            // we can check for character substitutions even though its expensive
            var analysis = info.frequencyAnalysis;

            char c1 = analysis[0].Char;
            char c2 = analysis[1].Char;
            char c3 = analysis.Length == 3 ? analysis[2].Char : ' ';

            List<List<char>> permutations = new(); DecryptionUtils.GetPermutations(new() { c1, c2, c3 }, new(), permutations);
            List<string> output = new(permutations.Count);

            foreach (List<char> perm in permutations)
            {
                output.Add(DecodeBinary(input, perm[0], perm[1], perm[2]));
            }

            return output;
        }
        public double CalculateProbability(string input, StringInfo info)
        {
            return info.uniqueCharacters > 3 ? 1 : 0;
        }

        public string Name { get { return "Binary"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
