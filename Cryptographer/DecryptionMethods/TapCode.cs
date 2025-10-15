using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class TapCode : IDecryptionMethod
    {

        private List<List<string>> TapDictionary = MethodDictionaries.TapCode;

        private string DecryptTap(string input, string space)
        {
            string modifiedInput = input.Replace(space, " ");
            string[] split = modifiedInput.Split(" ");

            // check string at each second space
            StringBuilder output = new();
            bool check = true;

            for (int i = 0; i < split.Length; i++)
            {
                check = !check;
                if (!check) continue;

                int curLength = split[i].Length;
                int prevLength = split[i - 1].Length;

                if (curLength > 5 || curLength <= 0) continue;
                if (prevLength > 5 || prevLength <= 0) continue;

                output.Append(TapDictionary[prevLength - 1][curLength - 1]);
            }

            return output.ToString();
        }

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            var analysis = info.frequencyAnalysis;

            // always second
            string space = analysis[1].Key.ToString();

            return new List<string>() {
                 DecryptTap(input, space)
            };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            var analysis = info.frequencyAnalysis;

            // no spaces just dots = analysis count 1
            return (analysis.Count != 2 ? 1 : 0.2); // you meet this less frequently than stuff like binary, morse, baconian.
        }

        public string Name { get { return "Tap Code"; } }
		public bool RequiresKey { get { return false; } }
    }
}
