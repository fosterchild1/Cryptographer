using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Runtime.Versioning;
using System.Text;

// placeholder
namespace Cryptographer.DecryptionMethods
{
    public class None : IDecryptionMethod
    {

        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            return new() { input };
        }

        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            return 1;
        }

        public string Name { get { return "None"; } }
    }
}
