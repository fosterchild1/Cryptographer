using Cryptographer.Classes;

namespace Cryptographer.Decoders
{
    public class ASCIIShift : IDecoder
    {
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            List<string> outputs = new(127);

            char[] shifted = new char[input.Length];
            for (int i=1; i<=127; i++)
            {
                int shiftedIdx = 0;

                foreach (char c in input)
                {
                    char shiftedChar = (char)((c + i) % 128);
                    shifted[shiftedIdx++] = shiftedChar;
                }

                outputs.Add(new(shifted));
            }

            return outputs;
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return 0.5;
        }

        public string Name { get { return "ASCII Shift"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return true; } }
    }
}
