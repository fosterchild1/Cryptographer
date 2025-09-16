using Cryptographer.Classes;
using Cryptographer.DecryptionMethods;
public class DecryptionBranch(DecryptionNode Parent, double Probability, IDecryptionMethod Method = default!)
{
    public IDecryptionMethod Method = Method ?? new None();
    public double Probability = Probability;
    public DecryptionNode Parent = Parent;

    public override string ToString()
    {
        return $"Method: {Method}, Probability: {Probability}";
    }
}
