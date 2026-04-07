public class DecryptionNode(string Text = "", byte Depth = 0, string Method = "", StringInfo info = default!, DecryptionNode? Parent = null)
{
    public readonly DecryptionNode? Parent = Parent;
    public readonly StringInfo info = info;
    public readonly string Text = Text;
    public readonly string Method = Method;
    public readonly byte Depth = Depth;

    public override string ToString()
    {
        
        return $"Text: {Text}, Depth: {Depth}, Method: {Method}";
    }
}