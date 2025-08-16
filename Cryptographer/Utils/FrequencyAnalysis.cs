class FrequencyAnalysis
{
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

}