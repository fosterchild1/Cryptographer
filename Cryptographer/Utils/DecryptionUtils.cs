namespace Cryptographer.Utils
{
    internal class DecryptionUtils
    {
        public static readonly List<string> EmptyResult = new();

        public static readonly HashSet<char> letterHashset = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToHashSet();

        public static readonly HashSet<char> numberHashset = @"0123456789".ToHashSet();

        public static void GetPermutations<T>(List<T> list, List<T> current, List<List<T>> permutations)
        {
            if (list.Count == 0)
            {
                permutations.Add(new(current)); // ugly
                return;
            }

            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];

                List<T> remaining = new(list);
                remaining.RemoveAt(i);

                current.Add(item);
                GetPermutations(remaining, current, permutations);
                current.RemoveAt(current.Count - 1);
            }
        }


        public static string RemoveWhitespaces(string input)
        {
            char[] buf = new char[input.Length];
            int idx = 0;

            foreach (char c in input)
            {
                if (char.IsWhiteSpace(c)) continue;
                buf[idx++] = c;
            }

            return new(buf, 0, idx);
        }

        public static bool IsContainedString(string input, string allowedChars)
        {
            foreach (char c in input)
            {
               if (allowedChars.IndexOf(c) == -1) return false;
            }

            return true;
        }

        /// <summary> Converts a string of numbers to another base. </summary>
        /// <param name="nums"></param>
        /// <param name="toBase">base to convert from</param>
        /// <returns>0 if failed</returns>
        public static char FromBase(string nums, int fromBase)
        {
            try
            {
                int val = Convert.ToInt32(nums, fromBase);
                return (char)val;
            }
            catch
            {
                return '\0';
            }
        }
    }
}
