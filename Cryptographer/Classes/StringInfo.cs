public struct CharCount(char c, int amount)
{
    public char Char = c;
    public int count = amount;
}

public class StringInfo // theres a C# class also called StringInfo but honestly i have no idea what else to name this
{
    public readonly CharCount[] frequencyAnalysis;
    public readonly int uniqueCharacters;
    public readonly char minChar = char.MaxValue;
    public readonly char maxChar = char.MinValue;

    public StringInfo(string str)
    {
        int[] analysis = new int[128];
        char[] used = new char[128];
        int usedCount = 0;

        // build analysis and get min chars and max chars
        foreach (char c in str)
        {
            minChar = (char)Math.Min(minChar, c);
            maxChar = (char)Math.Max(maxChar, c);
            if (c >= 128) continue;

            if (analysis[c] == 0)
                used[usedCount++] = c;

            analysis[c] += 1;
        }

        frequencyAnalysis = new CharCount[usedCount];
        uniqueCharacters = usedCount;

        // build then sort frequency analysis
        for (int i = 0; i < usedCount; i++)
        {
            char c = used[i];
            frequencyAnalysis[i] = new(c, analysis[c]);
        }

        Array.Sort(frequencyAnalysis, (a, b) => b.count.CompareTo(a.count));
    }

    /// <param name="candidates">The characters you want to search for. O(n).</param>
    /// <returns>A hashset of the candidates seen in the analysis.</returns>
    public HashSet<char> Exists(HashSet<char> candidates)
    {
        if (candidates == null) return new();

        HashSet<char> seen = new();
        foreach (CharCount cc in frequencyAnalysis)
        {
            char key = cc.Char;
            if (!candidates.Contains(key) || seen.Contains(key)) continue;
            seen.Add(key);
        }

        return seen;
    }

	/// <summary>Exists to replace checking the same thing with Regex.Replace, which is slow.<br/></summary>
	/// <param name="candidates">The characters you want to search for. O(n).</param>
	/// <returns>
	/// <see langword="true"/> if the string is only composed of those characters, excluding whitespaces;
	/// <see langword="false"/> otherwise.
	/// </returns>
	public bool IsExclusive(HashSet<char> candidates)
    {
        if (candidates == null) return new();

        foreach (CharCount cc in frequencyAnalysis)
        {
            // exclude whitespaces since encoded string is valid with them 99% of the time
            if (char.IsWhiteSpace(cc.Char)) continue;

            if (!candidates.Contains(cc.Char)) return false;
        }

        return true;
    }
}
