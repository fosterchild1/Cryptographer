class FrequencyAnalysis
{
    public static Dictionary<char, int> AnalyzeFrequency(string input)
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

        return analysis;
    }

}