namespace Cryptographer.DecryptionMethods
{
    public class Reverse : IDecryptionMethod
    {
        public string Decrypt(string input, SortedList<char, int> analysis)
        {           
            char[] letters = input.ToCharArray();
            Array.Reverse(letters);
            string output = new string(letters);

            return output;
        }

        public string Name { get { return "Reverse"; } }
    }
}
