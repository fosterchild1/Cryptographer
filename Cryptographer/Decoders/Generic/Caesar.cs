using Cryptographer.Classes;

namespace Cryptographer.Decoders
{
    public class Caesar : IDecoder
    {
        public List<string> Decrypt(string input, StringInfo info, string _)
        {
            List<string> output = new(25);

            char[] transformed = new char[input.Length];
            for (int i = 1; i <= 25; i++) // 26 = no transformation
            {
                int transIdx = 0;

                foreach (char c in input) 
                {
                    if (c >= 'A' && c <= 'Z')
                        transformed[transIdx++] = (char)(((c - 'A' + i) % 26) + 'A');

                    else if (c >= 'a' && c <= 'z')
                        transformed[transIdx++] = (char)(((c - 'a' + i) % 26) + 'a');

                    else
                        transformed[transIdx++] = c;
                }

                output.Add(new(transformed));
            }
            return output;
        }

        private HashSet<char> nonLetters = @"0123456789!@#$%^&*()-=_+[{]}|/?.>,<;:'""`~".ToHashSet();

        public double CalculateProbability(string input, StringInfo info)
        {
            // check if there arent any letters
            if (info.IsExclusive(nonLetters))
                return 1;

            return 0.4;
        }

        public string Name { get { return "Caesar"; } }
		public KeyLevel RequiredKey { get { return KeyLevel.NotKeyed; } }
		public bool IsFallback { get { return true; } }
    }
}
