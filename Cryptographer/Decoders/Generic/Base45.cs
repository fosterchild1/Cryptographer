using Cryptographer.Classes;
using Cryptographer.Utils;
using System.Text;

namespace Cryptographer.Decoders
{
    public class Base45 : IDecoder
    {
        private const string Base45Characters = MethodDictionaries.Base45Characters;

        private byte CharToVal(char c)
        {
            int i = Base45Characters.IndexOf(c);
            return (byte)i;
        }
        private bool FromBase45String(string input, out byte[] output)
        {
            List<byte> result = new();

            // pad
            input += new string('0', (3 - (input.Length % 3)) % 3);

            for (int i = 0; i < input.Length; i += 3)
            {
                //n=c+(d×45)+(e×45^2)
                byte c = CharToVal(input[i]);
                byte d = CharToVal(input[i + 1]);
                byte e = CharToVal(input[i + 2]);
                
                int n = c + (d * 45) + (e * 45 * 45);
                byte secondByte = (byte)((n >> 8) & 0xFF);

                if (secondByte != 0) result.Add(secondByte);
                result.Add((byte)(n & 0xFF));
            }

            output = result.ToArray();
            return true;
        }

        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            FromBase45String(input, out byte[] output);

            return new() { Encoding.UTF8.GetString(output) };
        }


        public double CalculateProbability(string input, StringInfo info)
        {
            // 45 chars
            if (info.uniqueCharacters <= 2 || info.uniqueCharacters > 45) return 1;
            if (info.minChar < '$' || info.maxChar > 'Z') return 1;

            return DecryptionUtils.IsContainedString(input, Base45Characters) ? 0 : 1;
        }

        public string Name { get { return "Base45"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return false; } }
    }
}
