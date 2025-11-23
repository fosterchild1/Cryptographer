/// <summary>
/// Provides info about a string such as frequency analysis in O(n log n) time.
/// </summary>
public class StringInfo // theres a C# class also called StringInfo but honestly i have no idea what else to name this
{
    public readonly List<KeyValuePair<char, int>> frequencyAnalysis;
    public readonly int uniqueCharacters;
    public readonly char minChar = char.MaxValue;
    public readonly char maxChar = char.MinValue;

    public StringInfo(string str)
    {
        Dictionary<char, int> analysis = new();
        foreach (char c in str)
        {
            analysis[c] = analysis.GetValueOrDefault(c) + 1;

            minChar = (char)Math.Min(minChar, c);
            maxChar = (char)Math.Max(maxChar, c);
        }

        frequencyAnalysis = analysis.OrderByDescending(pair => pair.Value).ToList();
        uniqueCharacters = frequencyAnalysis.Count;
    }

    /// <param name="candidates">The characters you want to search for</param>
    /// <returns>A hashset of the candidates seen in the analysis. O(n).</returns>
    public HashSet<char> Exists(HashSet<char> candidates)
    {
        if (candidates == null) return new();

        HashSet<char> seen = new();
        foreach (KeyValuePair<char, int> kvp in frequencyAnalysis)
        {
            if (!candidates.Contains(kvp.Key) || seen.Contains(kvp.Key)) continue;
            seen.Add(kvp.Key);
        }

        return seen;
    }
}
