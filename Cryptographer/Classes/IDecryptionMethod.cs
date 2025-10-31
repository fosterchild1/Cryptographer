public enum KeyLevel
{
    NotKeyed = 0,
    KeyedWithDefault = 1,
    Keyed = 2,
}

namespace Cryptographer.Classes
{
    public interface IDecryptionMethod
    {
        double CalculateProbability(string input, StringInfo info);
        List<string> Decrypt(string input, StringInfo info, string key = "");
        string Name { get; }
        KeyLevel RequiredKey { get; }

        bool IsFallback { get; }
    }
}