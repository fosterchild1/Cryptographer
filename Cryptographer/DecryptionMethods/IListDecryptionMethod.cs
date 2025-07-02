namespace Cryptographer.DecryptionMethods
{
    public interface IListDecryptionMethod
    {
        List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis);
        string Name { get; }
    }
}