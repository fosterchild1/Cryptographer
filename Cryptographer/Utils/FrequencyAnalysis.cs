class FrequencyAnalysis
{
    /// <returns>The frequency analysis of the input.</returns>
    public static List<KeyValuePair<char, int>> AnalyzeFrequency(string input)
    {
        Dictionary<char, int> analysis = new();

        foreach (char c in input)
        {
            analysis[c] = analysis.GetValueOrDefault(c) + 1;
        }

        List<KeyValuePair<char, int>> kvpAnalysis = analysis.OrderByDescending(pair => pair.Value).ToList();
        return kvpAnalysis;
    }

    /// <param name="analysis">The frequency analysis</param>
    /// <param name="candidates">The characters you want to search for</param>
    /// <returns>A hashset of the candidates seen in the analysis. O(n).</returns>
    public static HashSet<char> Exists(List<KeyValuePair<char, int>> analysis, HashSet<char> candidates)
    {
        if (analysis == null || candidates == null) return new();

        HashSet<char> seen = new();
        foreach (KeyValuePair<char, int> kvp in analysis)
        {
            if (!candidates.Contains(kvp.Key) || seen.Contains(kvp.Key)) continue;
            seen.Add(kvp.Key);
        }

        return seen;
    }
}