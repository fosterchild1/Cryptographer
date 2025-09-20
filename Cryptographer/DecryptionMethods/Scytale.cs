using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Scytale : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info)
        {
            // simple to bruteforce: all you need is one number 1 through CEIL(input len / 2)
            List<string> outputs = new();

            int length = input.Length;
            int end = length - 1;
            int maxLetters = (int)MathF.Ceiling(length / 2);

            // the amount you move each time is equal to FLOOR(input len / letters)
            // but when you warp back to the start you move 1 extra

            for (int i = 2; i < maxLetters; i++)
            {
                StringBuilder output = new();
                int move = (int)MathF.Floor(length / i);
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
    }
}
