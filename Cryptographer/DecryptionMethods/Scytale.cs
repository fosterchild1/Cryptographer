using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Scytale : IDecryptionMethod
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

            for (int move = 2; move < maxLetters; move++)
            {
                StringBuilder output = new();
                int pos = 0;

                while (output.Length < length)
                {
                    output.Append(input[pos]);

                    bool willWarp = (pos + move > end);
                    pos = pos + move - (willWarp ? end : 0);
                }

                outputs.Add(output.ToString());
            }

            return outputs;
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return 0.6; // i dont see it that much
        }

        public string Name { get { return "Scytale"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return true; } }
    }
}
