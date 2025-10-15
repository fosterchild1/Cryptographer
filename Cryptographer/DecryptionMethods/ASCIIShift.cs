using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class ASCIIShift : IDecryptionMethod
    {

        public List<string> Decrypt(string input, StringInfo info)
        {
            List<string> outputs = new();

            for (int i=1; i<=127; i++)
            {
                StringBuilder shifted = new();
                foreach (char c in input)
                {
                    char shiftedChar = (char)((c + i) % 128);
                    shifted.Append(shiftedChar);
                }

                outputs.Add(shifted.ToString());
            }

            return outputs;
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return 0.5;
        }

        public string Name { get { return "ASCII Shift"; } }
		public bool RequiresKey { get { return false; } }
    }
}
