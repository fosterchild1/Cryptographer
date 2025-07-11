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
                if (idx == -1) { output.Append(c); continue; }

                output.Append(layout2[idx]);
            }

            return output.ToString();
        }

        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            List<string> output = new();

            // ugly, layouts(layouts - 1), we cant find out what keyboard layout its using
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

        public string Name { get { return "Keyboard Substitution"; } }
    }
}
