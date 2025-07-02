namespace Cryptographer.DecryptionMethods
{
    public interface IListDecryptionMethod
    {
        List<string> Decrypt(string input, SortedList<char, int> analysis);
        string Name { get; }
    }
}