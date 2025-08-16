class DecryptionNode(string Text = "", byte Depth = 0, string Method = "", DecryptionNode? Parent = null)
{
    public string Text = Text;
    public byte Depth = Depth;
    public string Method = Method;
    public DecryptionNode? Parent = Parent;
    public List<DecryptionNode> Children = new();

    public void GetLeaves(List<string> results)
    {
        if (Children.Count == 0)
        {
            results.Add(Text);
            return;
        }

        foreach (DecryptionNode child in Children)
        {
            child.GetLeaves(results);
        }
    }

    public byte GetMaxDepth(byte max = 0)
    {
        if (Children.Count == 0)
        {
            return Math.Max(Depth, max);
        }

        foreach (DecryptionNode child in Children)
        {
            max = child.GetMaxDepth(max);
        }

        return max;
    }

    public override string ToString()
    {
        return $"Text: {Text}, Depth: {Depth}, Method: {Method}";
    }
}
