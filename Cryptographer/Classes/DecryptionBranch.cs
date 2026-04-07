using Cryptographer;

public readonly struct DecryptionBranch(DecryptionNode Parent, double Probability, byte MethodId = default!)
{
    public readonly double Probability = Probability;
    public readonly DecryptionNode Parent = Parent;
    public readonly byte MethodId = MethodId;

    public override string ToString()
    {
        return $"Method: {DecoderFactory.FromId(MethodId)}, Probability: {Probability}";
    }
}

