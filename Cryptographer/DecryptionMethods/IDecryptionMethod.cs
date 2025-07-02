namespace Cryptographer.DecryptionMethods
{
    public interface IDecryptionMethod
    {
        string Decrypt(string input, SortedList<char, int> analysis);
        string Name { get; }
    }
}