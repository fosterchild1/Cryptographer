using System.Text;

namespace Cryptographer.DecryptionMethods
{
    public class Base64 : IDecryptionMethod
    {
        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {
            Span<byte> buffer = new byte[input.Length];
            bool success = Convert.TryFromBase64String(input, buffer, out int bytes);

            if (!success)
                return new List<string>();
            
            return new List<string>() { Encoding.UTF8.GetString(buffer.Slice(0, bytes)) };
        }

        public string Name { get { return "Base64"; } }
    }
}
