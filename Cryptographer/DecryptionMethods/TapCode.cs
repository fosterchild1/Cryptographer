using Cryptographer.Utils;
using System.Runtime.Versioning;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class TapCode : IDecryptionMethod
    {

        private static List<List<string>> TapDictionary = MethodDictionaries.TapCode;

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

        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            // no spaces just dots
            if (analysis.Count != 2)
            {
                return new List<string>();
            }

            // always second
            string space = analysis[1].Key.ToString();

            return new List<string>() {
                 DecryptTap(input, space)
            };
        }

        public string Name { get { return "Tap Code"; } }
    }
}
