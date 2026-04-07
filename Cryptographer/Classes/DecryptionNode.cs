public class DecryptionNode(string Text = "", byte Depth = 0, string Method = "", StringInfo info = default!, DecryptionNode? Parent = null)
{
    public DecryptionNode? Parent = Parent;
    public StringInfo info = info;
    public string Text = Text;
    public string Method = Method;
    public byte Depth = Depth;

    public override string ToString()
    {
        return $"Text: {Text}, Depth: {Depth}, Method: {Method}";
    }
}