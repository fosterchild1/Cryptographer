using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Caesar : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info)
        {
            List<string> output = new();
            for (int i = 0; i <= 25; i++) // 26 = no transformation
            {
                StringBuilder transformed = new();
                foreach (char c in input) 
                {
                    if (c >= 'A' && c <= 'Z')
                        transformed.Append((char)(((c - 'A' + i) % 26) + 'A'));

                    else if (c >= 'a' && c <= 'z')
                        transformed.Append((char)(((c - 'a' + i) % 26) + 'a'));

                    else
                        transformed.Append(c);
                }

                output.Add(transformed.ToString());
            }

            return output;
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            return 0.2;
        }

        public string Name { get { return "Caesar"; } }
    }
}
