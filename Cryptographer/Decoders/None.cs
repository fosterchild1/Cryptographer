using Cryptographer.Classes;

// PLACEHOLDER
namespace Cryptographer.Decoders
{
    public class None : IDecoder
    {

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            return new() { input };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return 1;
        }

        public string Name { get { return "None"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
