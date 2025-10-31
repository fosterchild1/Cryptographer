using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Playfair : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info, string key)
        {
            // Decoding is vigenere but instead of text - key its key - text.
            StringBuilder output = new();

            for (int i = 1; i < input.Length; i+=2)
            {
                char c1 = input[i - 1];
                char c2 = input[i];
                if (c1 == c2) return new();

                int n1 = key.IndexOf(c1);
                int n2 = key.IndexOf(c2);
                if (n1 == -1 || n2 == -1) return new();

                // mega ugly alert
                int row1 = (int)MathF.Floor(n1 / 5);
                int row2 = (int)MathF.Floor(n2 / 5);

                int column1 = n1 % 5;
                int column2 = n2 % 5;

                if (row1 == row2)
                {
                    // shift row left
                    column1 = (column1 + 4) % 5;
                    column2 = (column2 + 4) % 5;
                }
                else if (column1 == column2)
                {
                    // shift column up
                    row1 = (row1 + 4) % 5;
                    row2 = (row2 + 4) % 5;
                }
                else
                {
                    // swap
                    (column1, column2) = (column2, column1);
                }

                output.Append(key[row1 * 5 + column1]);
                output.Append(key[row2 * 5 + column2]);
            }

            return new() { output.ToString() };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            if (input.Length % 2 != 0) return 1;

            return 0.2;
        }

        public string Name { get { return "Playfair"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.Keyed; } }
        public bool IsFallback { get { return true; } }
    }
}
