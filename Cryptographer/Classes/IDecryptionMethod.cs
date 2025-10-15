namespace Cryptographer.Classes
{
    public interface IDecryptionMethod
    {
        double CalculateProbability(string input, StringInfo info);
        List<string> Decrypt(string input, StringInfo info);
        string Name { get; }
        bool RequiresKey { get; }
    }
}