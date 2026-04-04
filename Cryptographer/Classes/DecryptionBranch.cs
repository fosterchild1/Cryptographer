using Cryptographer.Classes;
using Cryptographer.Decoders;
public class DecryptionBranch(DecryptionNode Parent, double Probability, StringInfo Info = default!, IDecoder Method = default!)
{
    public IDecoder Method = Method ?? new None();
    public double Probability = Probability;
    public DecryptionNode Parent = Parent;
    public StringInfo info = Info;

    public override string ToString()
    {
        return $"Method: {Method}, Probability: {Probability}";
    }
}

