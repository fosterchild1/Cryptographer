using Cryptographer.Classes;

// PLACEHOLDER
namespace Cryptographer.DecryptionMethods
{
    public class None : IDecryptionMethod
    {

        public List<string> Decrypt(string input, StringInfo info)
        {
            return new() { input };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return 1;
        }

        public string Name { get { return "None"; } }
    }
}
