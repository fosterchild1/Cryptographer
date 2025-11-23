using Cryptographer.Classes;

namespace Cryptographer.Decoders
{
    public class Reverse : IDecoder
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
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return true; } }
    }
}
