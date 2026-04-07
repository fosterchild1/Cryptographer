enum StringType
{
    GIBBERISH = 0,
    PLAINTEXT = 1,
    LINK = 2,
    CTF_FLAG = 3,
}

namespace Cryptographer.Utils
{
    internal class StringClassifier
    {
        // link stuff
        private static readonly List<string> prefixes = new() { "HTTPS:", "HTTP:" };
        private static readonly HashSet<string> subdomains = new() { "WWW", "API", "DEV", "DOCS", "STORE", "EN", "FR", "WIKI", "RO" };
        private static readonly HashSet<string> topdomains = new() { "COM", "NET", "ORG", "TV", "FR", "EN", "RO", "EDU", "GOV", "PRO", "LOL", "IO", "CO", "DEV" };
        private static readonly HashSet<string> extensions = new() { "HTM", "HTML", "PHP", "CSS", "JS", "JSON", "TXT" };

        // CTF
        private static readonly HashSet<string> CTFprefixes = new() { "FLAG", "CTF", "HTTB", "THM", "CTFLEARN", "PICOCTF", "DCTF" };
        private static readonly char[] CTFSymbols = { ':', '^', '-', '{' };

        // SEARCHER STUFF (makes more sense to be here tbh)
        public static bool IsValid(string output, string last)
        {
            // hasnt changed
            if (last == output)
                return false;

            if (DecryptionUtils.RemoveWhitespaces(output).Length <= 3)
                return false;

            // we have this to avoid building stringinfo which builds analysis too. its not needed here.
            char minChar = char.MaxValue;
            char maxChar = char.MinValue;
            bool singleCharacterString = true;

            foreach (char c in output)
            {
                minChar = (char)Math.Min(minChar, c);
                maxChar = (char)Math.Max(maxChar, c);

                if (minChar != c || maxChar != c)
                {
                    singleCharacterString = false;
                }
            }

            if (singleCharacterString)
                return false;

            // anything that's under SPACE, created mostly by running base methods on non-base encrypted inputs
            if (minChar < 32 && minChar != 10) // exclude line feed
                return false;

            // currently doesnt support base65536 or base2048 or those typa stuff
            if (maxChar > 127)
                return false;

            return true;
        }

        private static bool IsCTF(string input)
        {
            int index = input.IndexOfAny(CTFSymbols);
            if (index == -1)
                return false;

            string split = input.Substring(0, index);
            if (split != null && CTFprefixes.Contains(split))
                return true;

            return false;
        }

        private static bool IsLink(string input)
        {
            int score = 0;

            bool hasHttps = input.Contains("://"); // evil https hack
            
            string[] split = input.Split("./".ToCharArray());
            int splitLength = split.Length;

            // prefixes
            if (hasHttps && prefixes.Contains(split[0]))
                score += 5;

            int splitStart = hasHttps ? 2 : 0;

            // subdomains (www.)
            string subdomain = split[splitStart];
            if (subdomains.Contains(subdomain))
                score += 2;

            // topdomains (.com)
            if ((splitLength >= 2 && topdomains.Contains(split[splitStart + 1])) || (splitLength >= 3 && topdomains.Contains(split[splitStart + 2])))
                score += 4;

            // extensions (.html)
            string extension = split[split.Length - 1];
            if (extensions.Contains(extension))
                score += 7;

            return score > 8;

        }

        private static double LettersToSymbolsRatio(StringInfo info)
        {
            double symbols = 0; double letters = 0;

            foreach (CharCount cc in info.frequencyAnalysis)
            {
                if (DecryptionUtils.letterHashset.Contains(cc.Char))
                {
                    letters += cc.count;
                    continue;
                }

                symbols += cc.count;
            }

            return symbols / letters;
        }

        private static float CalculateScoreForDict(string input, int step, Dictionary<string, float> dict)
        {
            // how this works: we have a window of either 4 or 3 characters and we check for those in a dictionry
            // each string has a score, if it doesnt then it gets penalized proportional to the string length
            // if it has a score, add its score proportional to the string length
            // we also keep track of seen strings and if we have seen those before we also penalize
            if (dict == null) return 0;

            int length = input.Length;
            float substrIncrement = length / 4f;

            float score = 0;
            float exitScore = -(length * 6);

            int capacity = length / (step - 1);
            Dictionary<string, short> seen = new(capacity);

            for (int i = 0; i < length; i++)
            {
                string substr = input.Substring(i, Math.Min(step, length - i));

                if (!dict.TryGetValue(substr, out float val))
                {
                    score -= length; // penalize by str length
                    if (score < exitScore) return 0;

                    continue;
                }

                short substrSeenAmount = (short)(seen.GetValueOrDefault(substr) + 1);

                seen[substr] = substrSeenAmount;
                score += val * substrIncrement / (substrSeenAmount * 2);
            }

            return score;
        }

        public static StringType Classify(string input, StringInfo info)
		  {
            if (info.uniqueCharacters > 0 && info.uniqueCharacters <= 3)
                return StringType.GIBBERISH;

            input = input.ToUpper();

            // check other types of text
            if (IsLink(input))
                return StringType.LINK;
            else if (IsCTF(input))
                return StringType.CTF_FLAG;

            // calculate score
            string modifiedInput = DecryptionUtils.RemoveWhitespaces(input);
            double symbolRatio = LettersToSymbolsRatio(info);
            if (symbolRatio > 3.0 / 4) return StringType.GIBBERISH;

            // TODO: refactor this trigram calculation stuff
            bool tri = Config.useTrigrams || symbolRatio >= (double)1/60 * input.Length || input.Length <= 6;

            float score = CalculateScoreForDict(modifiedInput, (tri ? 3 : 4), (tri ? Ngrams.trigrams! : Ngrams.quadgrams!));
            return (score >= Config.scorePrintThreshold ? StringType.PLAINTEXT : StringType.GIBBERISH);
        }
    }
}
