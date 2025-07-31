namespace Cryptographer.DecryptionMethods
{
    public interface IDecryptionMethod
    {
        List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis);
        string Name { get; }
    }
}