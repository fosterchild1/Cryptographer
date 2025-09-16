public class DecryptionNode(string Text = "", byte Depth = 0, string Method = "", DecryptionNode? Parent = null)
{
    public string Text = Text;
    public byte Depth = Depth;
    public string Method = Method;
    public DecryptionNode? Parent = Parent;

    public override string ToString()
    {
        return $"Text: {Text}, Depth: {Depth}, Method: {Method}";
    }
}
