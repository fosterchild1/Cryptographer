using System.Text;

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
            StringBuilder sb = new(input, input.Length);

            int i = 0;
            while (i < sb.Length)
            {
                if (!char.IsWhiteSpace(sb[i]))
                {
                    i++;
                    continue;
                }

                sb.Remove(i, 1);
            }

            return sb.ToString();
        }
    }
}
