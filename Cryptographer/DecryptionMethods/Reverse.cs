using Cryptographer.Classes;

namespace Cryptographer.DecryptionMethods
{
    public class Reverse : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info, string _)
        {           
            char[] letters = input.ToCharArray();
            Array.Reverse(letters);
            string output = new(letters);

            return new List<string>() { output };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return 0.5;
        }

        public string Name { get { return "Reverse"; } }
		public bool RequiresKey { get { return false; } }
		public bool IsFallback { get { return false; } }
    }
}
