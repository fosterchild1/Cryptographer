namespace Cryptographer.DecryptionMethods
{
    public class Reverse : IDecryptionMethod
    {
        public string Decrypt(string input)
        {           
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be empty", nameof(input));
            }

            
            char[] letters = input.ToCharArray();
            Array.Reverse(letters);
            string output = new string(letters);

            return output;
        }

        public string Name { get { return "Reverse"; } }
    }
}
