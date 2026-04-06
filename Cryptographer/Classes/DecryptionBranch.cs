using Cryptographer.Classes;
using Cryptographer.Decoders;
public readonly struct DecryptionBranch(DecryptionNode Parent, double Probability, StringInfo Info = default!, IDecoder Method = default!)
{
    public readonly double Probability = Probability;
    public readonly DecryptionNode Parent = Parent;
    public readonly IDecoder Method = Method ?? new None();
    public readonly StringInfo info = Info;

    public override string ToString()
    {
        return $"Method: {Method}, Probability: {Probability}";
    }
}

