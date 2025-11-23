using Cryptographer.Classes;
using Cryptographer.Decoders;
public class DecryptionBranch(DecryptionNode Parent, double Probability, IDecoder Method = default!)
{
    public IDecoder Method = Method ?? new None();
    public double Probability = Probability;
    public DecryptionNode Parent = Parent;

    public override string ToString()
    {
        return $"Method: {Method}, Probability: {Probability}";
    }
}

