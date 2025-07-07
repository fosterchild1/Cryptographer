namespace Cryptographer.DecryptionMethods
{
    public class Reverse : IDecryptionMethod
    {
        public List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis)
        {           
            char[] letters = input.ToCharArray();
            Array.Reverse(letters);
            string output = new string(letters);

            return new List<string>() { output };
        }

        public string Name { get { return "Reverse"; } }
    }
}
