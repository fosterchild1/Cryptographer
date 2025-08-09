using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base64 : IDecryptionMethod
    {
        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            Span<byte> buffer = new byte[input.Length];
            Convert.TryFromBase64String(input, buffer, out int bytes);
            return new List<string>() { Encoding.UTF8.GetString(buffer.Slice(0, bytes)) };
        }

        public double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis)
        {
            // Binary and stuff
            if (analysis.Count <= 2) return 1;

            Span<byte> buffer = new byte[input.Length];
            return !Convert.TryFromBase64String(input, buffer, out int bytes2) ? 1 : 0;
        }
        public string Name { get { return "Base64"; } }
    }
}
