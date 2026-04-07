using Cryptographer;

public readonly struct DecryptionBranch(DecryptionNode Parent, byte MethodId = default!)
{
    public readonly DecryptionNode Parent = Parent;
    public readonly byte MethodId = MethodId;

    public override string ToString()
    {
        return $"Method: {DecoderFactory.FromId(MethodId)}";
    }
}

