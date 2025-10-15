using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Vigenere : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info, string key)
        {
            StringBuilder output = new();

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return 0.1;
        }

        public string Name { get { return "Vigenere"; } }
		public bool RequiresKey { get { return true; } }
    }
}
