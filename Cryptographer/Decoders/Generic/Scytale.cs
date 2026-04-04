using Cryptographer.Classes;

namespace Cryptographer.Decoders
{
    public class Scytale : IDecoder
    {
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            // simple to bruteforce: all you need is one number 1 through CEIL(input len / 2)
            List<string> outputs = new();

            int length = input.Length;
            int end = length - 1;
            int maxLetters = (int)MathF.Ceiling(length / 2);

            // solution that doesnt require a 2d matrix
            // the amount you move each time is equal to FLOOR(input len / letters)
            // but when you warp back to the start you move 1 extra

            char[] output = new char[input.Length];

            for (int move = 2; move < maxLetters; move++)
            {
                int outputIdx = 0;
                int pos = 0;

                while (outputIdx < length)
                {
                    output[outputIdx++] = input[pos];

                    bool willWarp = (pos + move > end);
                    pos = pos + move - (willWarp ? end : 0);
                }

                outputs.Add(new(output));
            }

            return outputs;
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return 0.4; // same as caesar because theres no way to detect it
        }

        public string Name { get { return "Scytale"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return true; } }
    }
}
