class FrequencyAnalysis
{
    public static List<KeyValuePair<char, int>> AnalyzeFrequency(string input)
    {
        Dictionary<char, int> analysis = new();

        foreach (char c in input)
        {
            if (!analysis.ContainsKey(c))
            {
                analysis[c] = 1;
                continue;
            }
            analysis[c] += 1;
        }

        List<KeyValuePair<char, int>> kvpAnalysis = analysis.OrderByDescending(pair => pair.Value).ToList();
        return kvpAnalysis;
    }

}