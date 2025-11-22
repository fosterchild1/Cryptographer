namespace Cryptographer.Utils
{
    internal class DecryptionUtils
    {
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
    }
}
