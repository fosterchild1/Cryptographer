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

    /// <param name="candidates">The characters you want to search for. O(n).</param>
    /// <returns>A hashset of the candidates seen in the analysis.</returns>
    public HashSet<char> Exists(HashSet<char> candidates)
    {
        if (candidates == null) return new();

        HashSet<char> seen = new();
        foreach (KeyValuePair<char, int> kvp in frequencyAnalysis)
        {
            char key = kvp.Key;
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

        foreach (KeyValuePair<char, int> kvp in frequencyAnalysis)
        {
            // exclude whitespaces since encoded string is valid with them 99% of the time
            if (char.IsWhiteSpace(kvp.Key)) continue;

            if (!candidates.Contains(kvp.Key)) return false;
        }

        return true;
    }
}
