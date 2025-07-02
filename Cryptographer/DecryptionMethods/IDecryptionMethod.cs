namespace Cryptographer.DecryptionMethods
{
    public interface IDecryptionMethod
    {
        string Decrypt(string input, List<KeyValuePair<char, int>> analysis);
        string Name { get; }
    }
}