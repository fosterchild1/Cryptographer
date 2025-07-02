class FrequencyAnalysis
{
    public static SortedList<char, int> AnalyzeFrequency(string input)
    {
        SortedList<char, int> analysis = new();

        foreach (char c in input)
        {
            if (!analysis.ContainsKey(c))
            {
                analysis[c] = 1;
                continue;
            }
            analysis[c] += 1;
        }

        analysis.Order();
        return analysis;
    }

}