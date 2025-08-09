namespace Cryptographer.DecryptionMethods
{
    public interface IDecryptionMethod
    {
        double CalculateProbability(string input, List<KeyValuePair<char, int>> analysis);
        List<string> Decrypt(string input, List<KeyValuePair<char, int>> analysis);
        string Name { get; }
    }
}