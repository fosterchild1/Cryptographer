using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class KeyboardSubstitution : IDecryptionMethod
    {

        // try as many different layouts without having too many outputs, AZERTY and QWERTZ only have 1 character difference
        private List<string> Layouts = new() {
            MethodDictionaries.QwertyLayout,
            MethodDictionaries.DvorakLayout,
            MethodDictionaries.ColemakLayout,
            MethodDictionaries.WorkmanLayout,
        };

        private string SubstituteLayout(string input, string layout, string layout2)
        {
            // split at each space
            StringBuilder output = new();
            foreach (char c in input.ToCharArray())
            {
                int idx = layout.IndexOf(c);
                if (idx == -1) return "";

                output.Append(layout2[idx]);
            }

            return output.ToString();
        }

        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            List<string> output = new();

            // uuuuuuugly
            foreach (string layout in Layouts)
            {
                foreach (string layout2 in Layouts)
                {
                    if (layout == layout2) { continue; }

                    output.Add(SubstituteLayout(input, layout, layout2));
                }
            }

            return output;
        }

        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            // this means its more likely to be morse, bacon or binary
            if (analysis.Count <= 3) return 1;

            return 0.85;
        }

        public string Name { get { return "Keyboard Substitution"; } }
    }
}
