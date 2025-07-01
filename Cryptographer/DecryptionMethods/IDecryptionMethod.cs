namespace Cryptographer.DecryptionMethods
{
    public interface IDecryptionMethod
    {
        string Decrypt(string input);
        string Name { get; }
    }
}