using Cryptographer.Classes;
using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base64 : IDecryptionMethod
    {
        public List<string> Decrypt(string input, StringInfo info)
        {
            Span<byte> buffer = new byte[input.Length];
            Convert.TryFromBase64String(input, buffer, out int bytes);

            string output = Encoding.UTF8.GetString(buffer.Slice(0, bytes));
            return new() { output };
        }

        public double CalculateProbability(string input, StringInfo info)
        {
            // 64 + padding character
            if (info.uniqueCharacters <= 2 || info.uniqueCharacters > 65) return 1;

            Span<byte> buffer = new byte[input.Length];
            return !Convert.TryFromBase64String(input, buffer, out int bytes2) ? 1 : 0;
        }
        public string Name { get { return "Base64"; } }
    }
}
